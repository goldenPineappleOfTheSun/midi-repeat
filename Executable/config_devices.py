import pygame.midi
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
selected_file = options[selected]
with open(selected_file, "r") as file:
    config = json.load(file)

# --- set ---

pygame.midi.init()
device_count = pygame.midi.get_count()
all_devices = [pygame.midi.get_device_info(i) for i in range(device_count)]
input_devices = [x[1].decode() for x in [device for device in all_devices if device[2] > 0]]
    
new_wiring = []
similar_devices = {}

for device in input_devices:
    if 'loopMIDI' in device:
        continue
    if device in similar_devices:
        similar_devices[device] += 1
    else:
        similar_devices[device] = 1

input_devices = []
for k, v in similar_devices.items():
    if v == 1:
        input_devices.append(k)
    else:
        for i in range(v):
            letter = chr(i + 65)
            input_devices.append(f'{k} [{letter}]')

for data in config['wiring']:
    name = data['name']
    print(f'Выбери устройство, которое будет играть "{name}"')

    for i, device in enumerate(input_devices):
        print(f'{i+1}: {device}')
    selected = int(input()) - 1
    new_wiring.append({
        "name": name,
        "device": input_devices[selected]
    })

config['wiring'] = new_wiring
with open(selected_file, "w") as file:
    json.dump(config, file, ensure_ascii=False, indent=4)

# --- test ---

devices = []
for line in config['wiring']:
    devices.append(line['device'])
wiring = '|'.join(devices).replace(' ', '%20')

scripts = []
for index in range(len(devices)):
    scripts.append(f'monitor.{index + 1}')
scripts = '>'.join(scripts)

socket = client.create()
time.sleep(1)

client.send(socket, f'set-basics 10000 16')
time.sleep(0.5)
client.send(socket, f'set-wiring {wiring}')
time.sleep(0.5)
client.send(socket, f'set-scripts {scripts}')
time.sleep(0.5)
print('Please wait...')
client.send(socket, f'start')
time.sleep(0.5)
print('''
..####....####...##..##..##..##..#####..        
.##......##..##..##..##..###.##..##..##.        
..####...##..##..##..##..##.###..##..##.        
.....##..##..##..##..##..##..##..##..##.        
..####....####....####...##..##..#####..        
........................................        
..####...##..##..######...####...##..##....##...
.##..##..##..##..##......##..##..##.##.....##...
.##......######..####....##......####......##...
.##..##..##..##..##......##..##..##.##..........
..####...##..##..######...####...##..##....##...
................................................
''')
time.sleep(0.5)
input()
client.send(socket, f'stop')
time.sleep(0.5)
input()