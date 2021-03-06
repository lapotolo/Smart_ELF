import * as matcher from '../../src/matcher';
import * as testUtil from '../testUtil';
import { DataObject } from '../../src/kb';

let opt = testUtil.parseOptions(process.argv);
const  faketime = new Date();

const dataset: DataObject[] = [
    {
        _meta: { idSource: '', tag: '', TTL: 0, reliability: 0, creationTime: faketime }, _id: 0,
        _data: { nome: 'pino', cognome: 'albero', titolo: { tipo: 'diploma', grado: 'scuolamedia' } }
    },
    {
        _meta: { idSource: '', tag: '', TTL: 0, reliability: 0, creationTime: faketime }, _id: 1,
        _data: { nome: 'lino', cognome: 'tessuto', titolo: { tipo: 'diploma', grado: 'scuolasuperiore' }, residenza: 'via seta' }
    },
    {
        _meta: { idSource: '', tag: '', TTL: 0, reliability: 0, creationTime: faketime }, _id: 2,
        _data: { nome: 'pino', cognome: 'radice', titolo: { tipo: 'laurea', grado: 'magistrale', voto: '45' } }
    },
    {
        _meta: { idSource: '', tag: '', TTL: 0, reliability: 0, creationTime: faketime }, _id: 3,
        _data: { nome: 'dino', cognome: 'sauro', grado: { tipo: 'laurea', grado: 'magistrale', voto: '45' } }
    },
    {
        _meta: { idSource: '', tag: '', TTL: 0, reliability: 0, creationTime: faketime }, _id: 4,
        _data: { nome: 'gianni', cognome: 'sauro', grado: { tipo: 'laurea', grado: 'triennale', voto: '20', qi: '100' } }
    },
    {
        _meta: { idSource: '', tag: '', TTL: 0, reliability: 0, creationTime: faketime }, _id: 5,
        _data: { nome: 'gianni', cognome: 'gianni', grado: { gianni: 'nome', qi: '900' } }
    },
    {
        _meta: { idSource: '', tag: '', TTL: 0, reliability: 0, creationTime: faketime }, _id: 6,
        _data: { nome: 'gianni', cognome: 'pinotto', gianni: 'nome', qi: '900' }
    }
];

const matches = matcher.findMatches({ _data: { nome: 'pino', cognome: '$cognome' } }, dataset);
const answer: Map<object, object[]> = new Map<object, object[]>();
answer.set({
    _meta: { idSource: '', tag: '', TTL: 0, reliability: 0, creationTime: faketime }, _id: 0,
    _data: { nome: 'pino', cognome: 'albero', titolo: { tipo: 'diploma', grado: 'scuolamedia' } }
}, [{ '$cognome': 'albero' }]);
answer.set({
    _meta: { idSource: '', tag: '', TTL: 0, reliability: 0, creationTime: faketime }, _id: 2,
    _data: { nome: 'pino', cognome: 'radice', titolo: { tipo: 'laurea', grado: 'magistrale', voto: '45' } }
}, [{ '$cognome': 'radice' }]);

testUtil.test(matches, answer, opt.verbose);
