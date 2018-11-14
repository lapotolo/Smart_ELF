from urllib.parse import urlencode # For URL creation
import httplib2
from lxml import etree
import wave, sys, pyaudio

import json
import sys

sys.path.insert(0, '../SmartApp.KB/')

from kb import *

myID = register()



#print(queryBind({"text_f_audio": "$x"}))
###############################################################################################################################################

def make_audio(txt):

    # Mary server informations
    mary_host = "localhost"
    mary_port = "59125"

    with open ("en_in", "r") as f:
        txt = f.read()
    language_in="dfki-prudence"
    language_text = "en-GB"
    
    # Build the query
    query_hash = {"INPUT_TEXT": txt,
                  "INPUT_TYPE": "EMOTIONML", # Input text
                  "LOCALE": language_text,
                  "VOICE": language_in, # Voice informations  (need to be compatible)
                  "OUTPUT_TYPE": "AUDIO",
                  "AUDIO": "WAVE", # Audio informations (need both)
                  }

    query = urlencode(query_hash)


    # Run the query to mary http server
    h_mary = httplib2.Http()
    good_response = False

    for i in range(3):
        resp, content = h_mary.request("http://%s:%s/process?" % (mary_host, mary_port), "POST", query)

        #  Decode the wav file or raise an exception if no wav files
        if (resp["content-type"] == "audio/x-wav"):

            # Write the wav file
            f = open("output_wav.wav", "wb")
            f.write(content)
            f.close()
            good_response = True
            break

    if not good_response:
        raise Exception(content)

    wf = wave.open('output_wav.wav')
    p = pyaudio.PyAudio()
    chunk = 1024
    stream = p.open(format =
                    p.get_format_from_width(wf.getsampwidth()),
                    channels = wf.getnchannels(),
                    rate = wf.getframerate(),
                    output = True)
    data = wf.readframes(chunk)
    while data != '':
        stream.write(data)
        data = wf.readframes(chunk)


# to do with arausand and valency
def generate_emotional_text(text, emotion="happy"):
    root = etree.Element('emotionml')
    root.attrib["version"] = "1.0"
    root.attrib["xmlns"] = "http://www.w3.org/2009/10/emotionml"
    root.attrib["category-set"] = "http://www.w3.org/TR/emotion-voc/xml#everyday-categories"
    child = etree.Element('emotion')
    category = etree.Element('category')
    category.attrib["name"] = emotion
    child.text = text
    root.append(child)
    child.append(category)
    return etree.tostring(root, pretty_print=True).decode("utf-8")



###############################################################################################################################################


def callbfun(res):


    print("callback:")

    print(res)
    make_audio("ciaoooooooooooooooo")

    print("\n waiting...")




subscribe(myID, {"TAG":"AV_IN_TRANSC_EMOTION","text": "$x","valence": "$v","arousal": "$a"}, callbfun)

print("\n waiting...")
