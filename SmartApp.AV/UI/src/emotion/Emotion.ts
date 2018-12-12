import * as Logger from '../log/Logger';
import { LinearCombiner } from '../utils/Combiners';
import { Point } from '../utils/Point';

export interface ISBEEmotion {
	getSadness(): number;
	getDisgust(): number;
	getAnger(): number;
	getSurprise(): number;
	getFear(): number;
	getHappiness(): number;
	getCalm(): number;
	getDefensive(): number;
	getThinking(): number;

	toString(): string;
}

export class SBEEmotion implements ISBEEmotion {

	constructor(protected sadness: number = 0,
		protected disgust: number = 0,
		protected anger: number = 0,
		protected surprise: number = 0,
		protected fear: number = 0,
		protected happiness: number = 0,
		protected calm: number = 0,
		protected defensive: number = null,
		protected thinking: number = null) { }

	public getSadness(): number {
		return this.sadness;
	}

	public getDisgust(): number {
		return this.disgust;
	}

	public getAnger(): number {
		return this.anger;
	}

	public getSurprise(): number {
		return this.surprise;
	}

	public getFear(): number {
		return this.fear;
	}

	public getHappiness(): number {
		return this.happiness;
	}

	public getCalm(): number {
		return this.calm;
	}

	public getDefensive(): number {
		return this.defensive;
	}

	public getThinking(): number {
		return this.thinking;
	}

	public toString(): string {
		return "["
			+ "sadness: " + this.sadness + ", "
			+ "disgust: " + this.disgust + ", "
			+ "anger: " + this.anger + ", "
			+ "surprise: " + this.surprise + ", "
			+ "fear: " + this.fear + ", "
			+ "fear: " + this.thinking + ", "
			+ "happiness: " + this.happiness
			+ "]"
	}
}

// 6 Basic emotions points of valence and arousal
const EMOTION_SADNESS = new Point(-0.75, -0.75);
const EMOTION_DISGUST = new Point(-0.90, 0.70);
const EMOTION_ANGER = new Point(-0.65, 0.35);
const EMOTION_SURPRISE = new Point(0.25, 0.57);
const EMOTION_FEAR = new Point(-0.60, 0.75);
const EMOTION_HAPPINESS = new Point(0.25, 0.75);
const EMOTION_CALM = new Point(0.1, -0.9);

/**
 * Function used: 1 - x^0.7
 * @param x 
 */
function computeBelonging(x: number): number {
	if (x > 1) {
		return 0;
	}
	return 1 - Math.pow(x, 0.7);
}

/**
 * This is an implementation of IEmotion based on trigonometry.
 */
export class ValenceArousalEmotion extends SBEEmotion {

	constructor(private valence: number, private arousal: number) {
		super();

		let p = new Point(valence, arousal);

		// TODO: check correctness of the computed values
		this.sadness = computeBelonging(EMOTION_SADNESS.distanceTo(p));
		this.disgust = computeBelonging(EMOTION_DISGUST.distanceTo(p));
		this.anger = computeBelonging(EMOTION_ANGER.distanceTo(p));
		this.surprise = computeBelonging(EMOTION_SURPRISE.distanceTo(p));
		this.fear = computeBelonging(EMOTION_FEAR.distanceTo(p));
		this.happiness = computeBelonging(EMOTION_HAPPINESS.distanceTo(p));
		this.calm = computeBelonging(EMOTION_CALM.distanceTo(p));
	}

	public getArousal(): number {
		return this.arousal;
	}

	public getValence(): number {
		return this.valence;
	}

	// public getLabel(): string {
	// 	let intensity = this.getIntensity();
	// 	if (intensity < 0.2) {
	// 		return "normal";
	// 	}

	// 	let angle = this.getAngle();

	// 	if (angle < 0) {
	// 		angle = 360 + angle;
	// 	}

	// 	// Emotions mapped into 6 basic ones
	// 	if (angle < 60.0) return EMOTION.HAPPINESS;
	// 	else if (angle < 120.0) return EMOTION.ANGER;
	// 	else if (angle < 180.0) return EMOTION.DISGUST;
	// 	else if (angle < 240.0) return EMOTION.SADNESS;
	// 	else if (angle < 300.0) return EMOTION.DISGUST;
	// 	else return EMOTION.SUPRISE;
	// }

	public toString(): string {
		return "["
			+ "valence: " + this.valence + ", "
			+ "arousal: " + this.arousal
			+ "]"
	}
}

export abstract class EmotionColorAdapter {
	abstract getColor(emotion: ISBEEmotion): string;

	static getAdapter(emotion: ISBEEmotion): EmotionColorAdapter {
		if (emotion instanceof ValenceArousalEmotion) {
			return new ValenceArousalEmotionColorAdapter();
		} else if (emotion instanceof SBEEmotion) {
			return new SBEEmotionColorAdapter();
		}

		Logger.getInstance().log(Logger.LEVEL.ERROR, "Emotion not recognized: ", emotion);
		return new DefaultColorAdapter();
	}
}

class DefaultColorAdapter extends EmotionColorAdapter {
	getColor(emotion: ISBEEmotion): string {
		return "#FFF";
	}
}

class ValenceArousalEmotionColorAdapter extends EmotionColorAdapter {
	public getColor(emotion: ValenceArousalEmotion): string {
		return this.getColorFromCoord(emotion.getValence(), emotion.getArousal());
	}

	/**
	 * Returns the hex color code out of valence and arousal values.
	 * This function map the HSL color to the Russell model, based on https://www.google.it/amp/s/cm4group.wordpress.com/2013/11/20/colors-and-emotions-by-p-zarc/amp/
	 * valence and arousal are between [0,1]
	 * The color is in HSL format: 
	 * 		hue = angle generated by valence and arousal (angle = 0 means yellow),
	 * 		saturation = the max possible saturation in order to have full color
	 * 		lightness = between 50% and 100%, in order to have from full color to white
	 * 
	 * @param valence The value of valence
	 * @param arousal the value of arousal
	 */
	private getColorFromCoord(valence: number, arousal: number): string {
		var hue = Math.atan2(-arousal, valence) * 180.0 / Math.PI;
		var dist = this.getIntensity(valence, arousal);
		return this.hslToHex(hue + 60, 100, 100 - dist * 50);
	}

	private getAngle(emotion: ValenceArousalEmotion): number {
		return Math.atan2(emotion.getArousal(), emotion.getValence()) * 180.0 / Math.PI
	}

	private getIntensity(valence: number, arousal: number): number {
		return Math.sqrt(Math.pow(valence, 2) + Math.pow(arousal, 2));
	}

	private hslToHex(h: number, s: number, l: number): string {
		h /= 360;
		s /= 100;
		l /= 100;
		let r, g, b;
		if (s === 0) {
			r = g = b = l; // achromatic
		} else {
			const hue2rgb = (p, q, t) => {
				if (t < 0) t += 1;
				if (t > 1) t -= 1;
				if (t < 1 / 6) return p + (q - p) * 6 * t;
				if (t < 1 / 2) return q;
				if (t < 2 / 3) return p + (q - p) * (2 / 3 - t) * 6;
				return p;
			};
			const q = l < 0.5 ? l * (1 + s) : l + s - l * s;
			const p = 2 * l - q;
			r = hue2rgb(p, q, h + 1 / 3);
			g = hue2rgb(p, q, h);
			b = hue2rgb(p, q, h - 1 / 3);
		}
		const toHex = x => {
			const hex = Math.round(x * 255).toString(16);
			return hex.length === 1 ? '0' + hex : hex;
		};
		return `#${toHex(r)}${toHex(g)}${toHex(b)}`;
	}
}

class SBEEmotionColorAdapter extends EmotionColorAdapter {
	public getColor(emotion: SBEEmotion): string {
		let r = (Math.random() * 100) % 255,
			g = (Math.random() * 100) % 255,
			b = (Math.random() * 100) % 255; // TODO: combine values to get the color!
		return `rgb(${r}, ${g}, ${b})`;
	}
}

/**
 * Returns a neutral emotion.
 */
export function getNeutral(): ISBEEmotion {
	return new ValenceArousalEmotion(0, 0);
}