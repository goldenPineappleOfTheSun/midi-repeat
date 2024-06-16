import client
import random
import json
import time
import re
import os

selected = ''
selected_file = ''

def question(text, options):
    global selected
    print(text)

    for key in options.keys():
        if key:
            print(f'{key}: {options[key]}')

    answer = 'NOT THE ANSWER!!!'
    while not answer in options.keys():
        answer = input()
    selected = answer
    print('-------')

# --- select configuration ---

options = dict()
for i, file in enumerate([x for x in os.listdir('./') if x.endswith(".midiconf")]):
    options[str(i + 1)] = file
question('select:', options)

# --- read configs ---

config = {}
with open(options[selected], "r") as file:
    config = json.load(file)

size = config['basic']['size']
beats = config['basic']['beats']

wiring = []
for line in config['wiring']:
    wiring.append(line['device'])
wiring = '|'.join(wiring).replace(' ', '%20')

scripts = '/'.join([x.replace(', ', '>').replace(',', '>').replace(' ', '.') for x in config['scripts']])

practice = False
question('Включить режим практики? (Enter - пропустить)', {'y':True, 'n':False, '':False})
if selected == 'y':
    question('Долей на удар метронома (скорость)?', {'1':1, '2':2, '4':4, '8':8})
    practice = selected

# --- start ---

socket = client.create()
time.sleep(1)
if config['backing'] and config['backing']['enabled']:
    backing_file = config['backing']['file'].replace(' ', '%20')
    backing_offset = config['backing']['offset']
    backing_volume = config['backing']['volume']
    backing_skiploops = config['backing']['skiploops']
    client.send(socket, f'prepare-backing-track {backing_file} {backing_offset} {backing_skiploops} {backing_volume}')
    time.sleep(2)

client.send(socket, f'set-basics {size} {beats}')
time.sleep(0.5)
client.send(socket, f'set-wiring {wiring}')
time.sleep(0.5)
client.send(socket, f'set-scripts {scripts}')
time.sleep(0.5)
if practice:
    client.send(socket, f'enable-practice {practice}')
    time.sleep(0.5)
if 'scheme' in config:
    scheme = config['scheme']
    if config['backing'] and config['backing']['skiploops']:
        skiploop = config['backing']['skiploops']
        scheme = '|'.join(scheme.split('|')[skiploop:])
    client.send(socket, f'set-scheme {scheme}')
    time.sleep(0.5)
if 'scheme_description' in config:
    scheme_description = config['scheme_description'].replace(' ', '%20')
    client.send(socket, f'set-scheme-description {scheme_description}')
    time.sleep(0.5)
client.send(socket, f'start')

print('нажми Enter, чтобы безопасно завершить работу')
input()
client.send(socket, f'stop')