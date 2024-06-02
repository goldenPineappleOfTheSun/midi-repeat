import client
import time
import random
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

lines = []
with open(options[selected], "r") as file:
    lines = [line.rstrip() for line in file]

wires = []
basic = []
scripts = []
buttons = ''
backing = []

state = 'wires'
states = ['wires', 'basic', 'scripts', 'buttons', 'backing', '']
for line in lines:
    if line == '':
        state = states[states.index(state) + 1]
        continue
    if state == 'wires':
        wires.append(line)
    if state == 'basic':
        basic.append(line)
    if state == 'scripts':
        scripts.append(line)
    if state == 'buttons':
        buttons = line
    if state == 'backing':
        backing.append(line)

# --- interpret configs ---

size = 0
beats = 0
for line in basic:

    match = re.search(r'size: (\d+)ms', line)
    if match:
        size = match.group(1)

    match = re.search(r'beats: (\d+)$', line)
    if match:
        beats = match.group(1)

wiredevices = []
for line in wires:
    match = re.search(r'\w+: ([\d\w ]+)$', line)
    wiredevices.append(match.group(1))
wires_encoded = '|'.join(wiredevices).replace(' ', '%20')

scripts_encoded = '/'.join([x.replace(', ', '>').replace(',', '>').replace(' ', '.') for x in scripts])

buttons_encoded = buttons.replace(' ', '%20')

backing_enabled = False
backing_file = ''
backing_offset = 0
if backing[0] == 'backing: true' or backing[0] == 'backing:true':
    backing_enabled = True
    for line in backing:

        match = re.search(r'file: "([\w\s\.\d_-]+)"', line)
        if match:
            backing_file = match.group(1).replace(' ', '%20')
        
        match = re.search(r'offset: (\d+)', line)
        if match:
            backing_offset = match.group(1)

# --- start ---

socket = client.create()
time.sleep(1)
if backing_enabled:
    client.send(socket, f'prepare-backing-track {backing_file} {backing_offset}')
    time.sleep(2)

client.send(socket, f'set-basics {size} {beats}')
time.sleep(0.5)
client.send(socket, f'set-wiring {wires_encoded}')
time.sleep(0.5)
client.send(socket, f'set-scripts {scripts_encoded}')
time.sleep(0.5)
client.send(socket, f'set-buttons {buttons_encoded}')
time.sleep(0.5)
client.send(socket, f'start')

input()
client.send(socket, f'stop')