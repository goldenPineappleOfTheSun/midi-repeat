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
        print(f'{key}: {options[key]}')

    answer = ''
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

# --- start ---

socket = client.create()
time.sleep(1)
if config['backing'] and config['backing']['enabled']:
    backing_file = config['backing']['file'].replace(' ', '%20')
    backing_offset = config['backing']['offset']
    client.send(socket, f'prepare-backing-track {backing_file} {backing_offset}')
    time.sleep(2)

client.send(socket, f'set-basics {size} {beats}')
time.sleep(0.5)
client.send(socket, f'set-wiring {wiring}')
time.sleep(0.5)
client.send(socket, f'set-scripts {scripts}')
time.sleep(0.5)
if 'scheme' in config:
    scheme = config['scheme']
    client.send(socket, f'set-scheme {scheme}')
    time.sleep(0.5)
client.send(socket, f'start')

input()
client.send(socket, f'stop')