from queue import Queue
from math import floor
import pygame.midi
import pyaudio  
import socket
import time
import wave  
import os
import re

#-- enums --
        
class tape_states:
    monitor = 'monitoring'
    mute = 'muted'
    record = 'recording'
    play = 'playing'
        
class server_states:
    created = 'created'
    pending = 'pending'
    run = 'run'
    debug_events = 'debug_events'
    terminated = 'terminated'

class bcolors:
    HEADER = '\033[95m'
    OKBLUE = '\033[94m'
    OKCYAN = '\033[96m'
    OKGREEN = '\033[92m'
    WARNING = '\033[93m'
    FAIL = '\033[91m'
    ENDC = '\033[0m'
    BOLD = '\033[1m'
    UNDERLINE = '\033[4m'

#-- classes --

class Note:
    def __init__(self, pos, pitch, volume, length = 0):
        self.pitch = pitch
        self.volume = volume
        self.pos = pos
        self.length = length
        self._play_started = 0
        
    def started(self, pos):
        self._play_started = pos
        
    def stoped(self):
        self._play_started = 0

    def pitch_str(self):
        # 48 = C
        names = ['C', 'C#', 'D', 'D#', 'E', 'F', 'F#', 'G', 'G#', 'A', 'A#', 'B']
        return names[self.pitch % 12]

    def __str__(self):
        return f'[♪{self.pitch_str()} {round(self.pos)}ms {round(self.length)}ms{f" ({round(self._play_started)}ms)" if self._play_started > 0 else ""}]'

class MPD218Preprocessor:
    def __init__(self):
        self.saved = []
        self.treshold = 60

    def is_start_event(self, event):
        return event[0][0] == 153 or event[0][0] == 144

    def process_input(self, events):
        if len(events) == 0:
            return events

        timestamp = events[0][1]
        self.saved = [x for x in self.saved if timestamp - x[1] < self.treshold]
        
        result = []
        for event in events:
            ok = True
            for saved in self.saved:
                if not self.is_start_event(event):
                    ok = True
                    break
                if saved[0][0] == event[0][0] and saved[0][1] == event[0][1] and event[1] - saved[1] < self.treshold:
                    ok = False
                    break
            if ok:
                result.append(event)
                self.saved.append(event)
        
        return result

class BackingTrack:
    def __init__(self, filename, startloop = 0, offset = 0, volume = 1):
        self.pyaudio = pyaudio.PyAudio()  
        self.filename = filename
        self.first_loop = startloop
        self.offset = offset
        self.volume = volume
        self.file = None
        self.stream = None
        self.buffer = None
        self.chunk = 1024
        self.prepare(filename, offset)

    def prepare(self, filename, offset):
        self.file = wave.open(filename, "rb") 
        self.stream = self.pyaudio.open(format = self.pyaudio.get_format_from_width(self.file.getsampwidth()),  
            channels = self.file.getnchannels(),  
            rate = self.file.getframerate(),  
            output = True)  
        self.buffer = b'\x00' * self.chunk
        
        while self.offset < 0:
            self.offset += self.chunk
            self.buffer = self.file.readframes(self.chunk)

    def read(self):
        if self.offset > 0:
            self.offset -= self.chunk
            self.stream.write(b'\x00' * self.chunk)  
            return
        self.stream.write(self.buffer)  
        self.buffer = self.file.readframes(self.chunk)

'''
инициализация:
1. программа спрашивает, сколько будет дорожек (выходных портов)
2. для каждого порта программа выводит список доступных входных устройств
-. сущности связаны следующим правилом: [pygame.midi.Output] ←1-1← [tape] ←m-1← [device]
-. устройства никогда не повторяются (id примем равным входному midi-id)
-. тейпы тоже никогда не повторяются (id примем равным выходному midi-id)
3. в конце концов будет список на подобии [(0, 1), (1, 2), (0, 3), (0, 4), (4, 5)], то есть входные могут повторятся, а выходные не могут и идут по порядку
4. на его основе создаются устройства, в данном случае три штуки: 0, 1, 4
5. каждое устройство создвёт у себя кассеты, согласно списку, например 0 устройство создаст 1, 3 и 4
6. каждый тейп, получает при создании свой выходной айди и ссылку на девайс
7. затем создаёт pygame.midi.Output

сигналы:
1. устройство может принимать входные сигналы
2. каждый сигнал отправляется на обработку во все кассеты
3. если кассета мониторит, то она тут же отправляет сигнал на выход
4. иначе она может что-то ещё делать хз
'''

class Tape:
    '''
    out_port - global pygame midi index, for example 1
    port_index - tape number from 1 to 12 or "metronome"
    device - owner (Device)
    loop_length - length in ms
    '''
    def __init__(self, out_port, port_index, device, loop_length = 0):
        self.notes = []
        self.state = tape_states.mute
        self.prev_state = self.state
        self.pos = 0
        self.device = device
        self.out = out_port
        self.port_index = port_index
        self.player = pygame.midi.Output(out_port)
        self.current_real_notes = {}
        self.loop_length = loop_length
        self.play_after_record = False
        self.silent_record = False

    def is_note_physically_pressed(self, note):
        return note.pitch in self.current_real_notes and self.current_real_notes[note.pitch] != None
        
    def get_output_info(self):
        device_count = pygame.midi.get_count()
        all_devices = [pygame.midi.get_device_info(i) for i in range(device_count)]
        info = all_devices[self.out]
        return f'{info[1].decode()} ({info[0].decode()})'
        
    def note_on(self, note):
        self.player.note_on(note.pitch, note.volume)
        
    def note_off(self, note):
        self.player.note_off(note.pitch, note.volume)
        
    def process(self, pos, midi_events):
        self.process_physical_notes(pos, midi_events)
        if self.state == tape_states.play:
            self.process_play(pos)
        if self.state == tape_states.record:
            self.process_record(pos, midi_events)
        if self.state == tape_states.record or self.state == tape_states.monitor:
            self.process_monitor(pos, midi_events)
            
    def process_physical_notes(self, pos, midi_events):
        started = [x for x in midi_events if x[0][0] == 144 or x[0][0] == 153]
        stoped = [x for x in midi_events if x[0][0] == 128 or x[0][0] == 137]
        for event in started:
            pitch = event[0][1]
            if not pitch in self.current_real_notes:
                self.current_real_notes[pitch] = None
            self.current_real_notes[pitch] = event
        for event in stoped:
            pitch = event[0][1]
            self.current_real_notes[pitch] = None

    def process_monitor(self, pos, midi_events):
        started = [x for x in midi_events if x[0][0] == 144 or x[0][0] == 153]
        stoped = [x for x in midi_events if x[0][0] == 128 or x[0][0] == 137]

        for event in started:
            pitch = event[0][1]
            volume = event[0][2]
            note = Note(pos, pitch, volume)
            self.note_on(note)

        for event in stoped:
            pitch = event[0][1]
            volume = event[0][2]
            note = Note(pos, pitch, volume)
            self.note_off(note)

    def process_play(self, pos):
        for note in self.notes:
            if note.pos > pos:
                continue
            if note.pos >= self.pos and note.pos < pos:
                self.note_on(note)
                note.started(pos)
            if note._play_started > 0 and pos - note._play_started > note.length:
                self.note_off(note)
                note.stoped()
        self.pos = pos
            
    def process_record(self, pos, midi_events):
        for event in midi_events:
            is_on = event[0][0] == 144 or event[0][0] == 153
            is_off = event[0][0] == 128 or event[0][0] == 137
            pitch = event[0][1]
            volume = event[0][2]
            if is_on:
                self.notes.append(Note(pos, pitch, volume))
            if is_off:
                for note in [x for x in self.notes if x.length == 0 and x.pitch == pitch]:
                    length = pos - note.pos
                    if length < 0:
                        length = 0.1
                    note.length = length
                    
    def end_loop(self):
        self.prev_state = self.state

        if self.state == tape_states.play:
            for note in self.notes:
                if note._play_started > 0:
                    self.note_off(note)
                    note.stoped()

        if self.state == tape_states.record:
            print(f'{bcolors.OKCYAN}STOP RECORD{bcolors.ENDC}')
            if self.play_after_record:
                self.play()
            else:
                self.mute()

            self.play_after_record = False
            self.silent_record = False

            for note in self.notes:
                if note.length == 0:
                    note.length = self.loop_length - note.pos
                    self.note_off(note)

        self.device.socket_send('loop-event')
                    
    def start_loop(self):
        if self.prev_state == tape_states.monitor and self.state != tape_states.monitor:
            current_notes = {k:v for k, v in self.current_real_notes.items() if v != None}
            for event in current_notes.values():
                pitch = event[0][1]
                volume = event[0][2]
                note = Note(0, pitch, volume)
                self.note_off(note)

        if self.state == tape_states.record:
            current_notes = {k:v for k, v in self.current_real_notes.items() if v != None}
            for event in current_notes.values():
                pitch = event[0][1]
                volume = event[0][2]
                note = Note(0, pitch, volume)
                self.notes.append(note)
                self.note_on(note)

        self.pos = 0

    def set_loop_length(self, length):
        self.loop_length = length

    def play(self):
        self.state = tape_states.play
        self.device.socket_send(f'tape-play {self.port_index}')
        print(f'{bcolors.OKGREEN}PLAY [{self.port_index}]{bcolors.ENDC}')
            
    def record(self):
        self.state = tape_states.record
        self.device.socket_send(f'tape-record {self.port_index}')
        print(f'{bcolors.FAIL}RECORD [{self.port_index}]{bcolors.ENDC}')
            
    def shadow(self):
        self.state = tape_states.record
        self.silent_record = True
        self.device.socket_send(f'tape-shadow {self.port_index}')
        print(f'{bcolors.HEADER}RECORD [{self.port_index}]{bcolors.ENDC}')
            
    def repeat(self):
        self.state = tape_states.record
        self.play_after_record = True
        self.device.socket_send(f'tape-record {self.port_index}')
        print(f'{bcolors.FAIL}RECORD-REPEAT [{self.port_index}]{bcolors.ENDC}')
            
    def mute(self):
        self.state = tape_states.mute
        self.device.socket_send(f'tape-mute {self.port_index}')
        print(f'{bcolors.WARNING}MUTE [{self.port_index}]{bcolors.ENDC}')

    def monitor(self):
        self.state = tape_states.monitor
        self.device.socket_send(f'tape-monitor {self.port_index}')
        print(f'{bcolors.OKBLUE}MONITOR [{self.port_index}]{bcolors.ENDC}')

    def stop(self):
        current_notes = {k:v for k, v in self.current_real_notes.items() if v != None}
        for event in current_notes.values():
            self.note_off(note)
        for note in self.notes:
                self.note_off(note)
                note.stoped()

    def __str__(self):
        return self.get_output_info()

class Device:
    '''
    device_in - midi device id
    device_outs - array of tuples (A, B)
                  A - midi device id
                  B - port number from 1 to 12
    '''
    def __init__(self, device_in, device_outs, server):
        self.inp = device_in
        self.outs = [x[0] for x in device_outs]
        self.tapes = [Tape(x[0], int(x[1]), self) for x in device_outs]
        self.server = server
        
    def get_input_info(self):
        device_count = pygame.midi.get_count()
        all_devices = [pygame.midi.get_device_info(i) for i in range(device_count)]
        info = all_devices[self.inp]
        return f'{info[1].decode()} ({info[0].decode()})'

    def set_loop_length(self, length):
        for tape in self.tapes:
            tape.set_loop_length(length)
                    
    def process(self, pos, midi_events):
        for tape in self.tapes:
            tape.process(pos, midi_events)

    def end_loop(self):
        for tape in self.tapes:
            tape.end_loop()

    def start_loop(self):
        for tape in self.tapes:
            tape.start_loop()

    def socket_send(self, message):
        self.server.socket_send(message)

    def stop(self):
        for tape in self.tapes:
            tape.stop()

    def __str__(self):
        return '\n'.join([f'{self.get_input_info()} -> {tape}' for tape in self.tapes])

class ControlButton:
    def __init__(self, source):
        match = re.match(r'(.*) event-(\d+) note-(\d+)', source)
        self.name = match.group(1)
        self.event = int(match.group(2))
        self.note = int(match.group(3))

    '''
    pygame_midi_input - for example "pygame.midi.Input(2)"
    raw_event - for example "[[144, 53, 8, 0], 8168]"
    '''
    def check_event(self, pygame_midi_input, raw_event):
        name = get_input_device_name(pygame_midi_input.device_id)
        return name == self.name and raw_event[0][0] == self.event and raw_event[0][1] == self.note

class ScriptsCache:
    def __init__(self, server, source_code):
        self.scripts_handlers = {
            'msg': lambda text: lambda: self._msg(text),
            'play': lambda tape: lambda: self._play(tape),
            'record': lambda tape: lambda: self._record(tape),
            'repeat': lambda tape: lambda: self._repeat(tape),
            'shadow': lambda tape: lambda: self._shadow(tape),
            'mute': lambda tape: lambda: self._mute(tape),
            'monitor': lambda tape: lambda: self._monitor(tape),
            'metronome': lambda value: lambda: self._metronome(value),
        }
        self.scripts = []
        self.server = server
        self.staged_scripts = []
        self.scripts_source = source_code.split('/')
        self.parse_scripts(source_code)
        self.fired_count = 0

    def _msg(self, text):
        print(text)

    def _monitor(self, index):
        tape = self.server.find_tape(int(index))
        tape.monitor() 
        
    def _play(self, index):
        tape = self.server.find_tape(int(index))
        tape.play() 
        
    def _record(self, index):
        tape = self.server.find_tape(int(index))
        tape.record() 
        
    def _repeat(self, index):
        tape = self.server.find_tape(int(index))
        tape.repeat() 
        
    def _shadow(self, index):
        tape = self.server.find_tape(int(index))
        tape.shadow() 
        
    def _mute(self, index):
        tape = self.server.find_tape(int(index))
        tape.mute() 
        
    def _metronome(self, value):
        value = True if value == 'on' else False
        if value == True:
            self.server.enable_metronome()
            print(f'{bcolors.OKBLUE}METRONOME ON{bcolors.ENDC}')
        else:
            self.server.disable_metronome()
            print(f'{bcolors.OKBLUE}METRONOME OFF{bcolors.ENDC}')

    def parse_script(self, text):
        try:
            lines = text.split('>')
            actions = []
            for line in lines:
                args = line.split('.')
                command = args[0]
                args = args[1:]
                actions.append(self.scripts_handlers[command](*args))
            def result():
                for action in actions:
                    action()
            return result
        except Exception as ex:
            print(f'{bcolors.FAIL}ERROR while parsing scripts: {ex}{bcolors.ENDC}')

    def parse_scripts(self, text):
        lines = text.split('/')
        self.scripts = [self.parse_script(x) for x in lines]
        
    def stage_script(self, n):
        if n > len(self.scripts) - 1:
            print(f'{bcolors.FAIL}There is no script with index {n}{bcolors.ENDC}')
        self.staged_scripts.append(self.scripts[n])

    '''
    использовать с осторожностью
    вызов посередине проигрывающейся кассеты может привести к непредсказуемым последствиям
    '''
    def execute_immediately(self, n):
        self.scripts[n]()
        
    def fire_staged_scripts(self):
        for script in self.staged_scripts:
            self.fired_count += 1
            script()
        self.staged_scripts.clear()

    def get_script_source(self, index):
        return self.scripts_source[index]

class ServerData:
    def __init__(self):
        self.start_time = None # in seconds
        #self.loop_start = None # in seconds
        self.loop_length = None # in seconds
        self.beats = None # how many metronome ticks are in one loop
        #self.scripts_source = None # scripts as tring
        #self.scripts = None # array of lambdas
        self.buttons = None # ControlButton[]
        self.devices = [] # Device[]
        self.mailboxes = [] # pygame.midi.Input
        self.preprocessors = [] # process raw input events (raw_event[] -> raw_event[])

class Server:
    def __init__(self, host, port):
        self.socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.socket.bind((host, port))
        self.socket.listen(1)
        self.client_socket = None
        self.client_address = None
        self.state = server_states.created
        self.data = ServerData()
        self._current_time = 0
        self._last_current_time = 0
        self.debug_inputs = []
        self.scriptsCache = None
        self.metronome_output = None
        self.is_metronome_on = True
        self.song_started = False
        self._socket_send_generator = self._create_socket_send_generator()
        self.backing_track = None
        self.loops_count = 0
        print(f"{bcolors.OKBLUE}Server listening on {host}:{port}{bcolors.ENDC}")

    def accept(self):
        self.client_socket, self.client_address = self.socket.accept()
        self.client_socket.setblocking(False)
        self.state = server_states.pending

    def mainloop(self):
        data = ''

        next(self._socket_send_generator)

        if self.backing_track and self.state == server_states.run and self.loops_count > self.backing_track.first_loop:
            self.backing_track.read()

        try:
            data = self.client_socket.recv(1024).decode()
        except BlockingIOError:
            # no data
            if self.state == server_states.debug_events:
                self.process_debug_events()
            if self.state == server_states.run:
                self.process()
            return

        if data != "":
            print(f'Received: "{data}"')
            
        if data == 'exit':
            self.state = server_states.terminated
            self.stop_server()
            #raise Exception('Got exit message')
            
        message = data.split(' ')
        command = message[0]
        
        if command == 'check-wiring':
            self.check_wiring(message[1].replace('%20', ' ').split('|'))
            return
        
        if command == 'request-io':
            self.request_io()
            return
        
        if command == 'start-debug-events':
            self.start_debug_events(*message[1:])
            return
        
        if command == 'stop-debug-events':
            self.stop_debug_events(*message[1:])
            return

        if command == 'prepare-backing-track':
            self.prepare_backing_track(*message[1:])
            return
        
        if command == 'set-basics':
            self.set_basics(*message[1:])
            return
        
        if command == 'set-wiring':
            self.set_wiring(*message[1:])
            return
        
        if command == 'set-scripts':
            self.set_scripts(*message[1:])
            return
        
        if command == 'set-buttons':
            self.set_buttons(*message[1:])
            return
        
        if command == 'enable-metronome':
            self.is_metronome_on = True
            return
        
        if command == 'disable-metronome':
            self.is_metronome_on = False
            return
        
        if command == 'start':
            self.start(*message[1:])
            return

        if command == 'stop':
            self.stop()
            return
        
        if command == 'hard-reset-midi':
            self.stop() # or hard_reset_midi()
            return

    def check_wiring(self, inputs):
        device_count = pygame.midi.get_count()
        all_devices = [pygame.midi.get_device_info(i) for i in range(device_count)]
        input_devices = [x[1].decode() for x in [device for device in all_devices if device[2] > 0]]
        output_devices = [x[1].decode() for x in [device for device in all_devices if device[3] > 0]]
        error_message = ''
        same_devices_count = {}
        letters = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H']
        for i, inp in enumerate(input_devices):
            name = re.sub(r'(.*)( \[[A-H]\])$', r'\1', inp)
            if not name in same_devices_count:
                same_devices_count[name] = 0
            same_devices_count[name] += 1

        if not 'loopMIDI Port metronome' in output_devices:
            print('------NO METRONOME-------')
            error_message += 'no metronome\n'

        for i, inp in enumerate(inputs):
            if not f'loopMIDI Port {i + 1}' in output_devices:
                print(f'------NO OUTPUT loopMIDI Port {i + 1}-------')
                error_message += 'no output loopMIDI Port ' + str(i + 1) + '\n'

            match = re.match(r'.* \[([A-H])\]', inp)
            if match:
                name = re.sub(r'(.*)( \[[A-H]\])$', r'\1', inp)
                print(f'input: {inp}')
                print(f'wo: {name}')
                index = letters.index(match.group(1))
                if not name in same_devices_count or same_devices_count[name] < index + 1:
                    print(f'------NO INDEXED INPUT {inp}-------')
                    error_message += 'no input "' + inp + '"\n'
            else:
                if not inp in input_devices:
                    print(f'------NO INPUT {inp}-------')
                    error_message += 'no input "' + inp + '"\n'


            
            if not f'loopMIDI Port {i + 1}' in output_devices:
                print(f'------NO OUTPUT loopMIDI Port {i + 1}-------')
                error_message += 'no output loopMIDI Port ' + str(i + 1) + '\n'
            if not name in input_devices:
                print(f'------NO INPUT {inp}-------')
                error_message += 'no input "' + name + '"\n'
    
        if error_message != '':
            self.socket_send("callback-check-wiring " + error_message)
        else:
            self.socket_send('callback-check-wiring ok')

    def request_io(self):
        device_count = pygame.midi.get_count()
        all_devices = [pygame.midi.get_device_info(i) for i in range(device_count)]
        input_devices = [x[1].decode() for x in [device for device in all_devices if device[2] > 0]]
        output_devices = [x[1].decode() for x in [device for device in all_devices if device[3] > 0]]
        output_count = 0

        while f'loopMIDI Port {output_count + 1}' in output_devices:
            output_count += 1
        
        self.socket_send(f'callback-request-io outputs:{output_count};inputs:{"|".join(input_devices)}')

    def start_debug_events(self, message):
        self.state = server_states.debug_events
        
        device_count = pygame.midi.get_count()
        all_devices = [pygame.midi.get_device_info(i) for i in range(device_count)]
                    
        message = message.replace('%20', ' ').split('|')
        message = [x for x in message if x]
        for i, device in enumerate(all_devices):
            if device[2] > 0:
                self.debug_inputs.append(pygame.midi.Input(i))
                    
    def stop_debug_events(self):
        self.state = server_states.pending
        self.debug_inputs.clear()
        hard_reset_midi()
            
    def process_debug_events(self):
        midi_events = []
        for debug_input in self.debug_inputs:
            if debug_input.poll():
                midi_events = debug_input.read(10)
                for event in midi_events:
                    message = f'debug-event {get_input_device_name(debug_input.device_id)} event-{event[0][0]} note-{event[0][1]} ({event})'
                    print(message) # do not erase
                    self.socket_send(message)

    def prepare_backing_track(self, filename, startloop, offset):
        self.backing_track = BackingTrack(filename.replace('%20', ' '), int(startloop), int(offset))

    def set_basics(self, loop_size, beats):
        self.data.loop_length = int(loop_size)
        self.data.beats = int(beats)

    def set_wiring(self, message):
        self.data.devices.clear()
        self.data.mailboxes.clear()
        self.metronome_output = Tape(get_output_device_id('metronome'), 'metronome', self)
        message_devices = message.replace('%20', ' ').split('|')
        outputs = {}
        for i, device_name in enumerate(message_devices):
            if device_name not in outputs:
                outputs[device_name] = []
            output_id = get_output_device_id(i + 1)
            output_info = pygame.midi.get_device_info(output_id)
            outputs[device_name].append((output_id, i + 1))
        for device_name in set(message_devices):
            id = get_input_device_id(device_name)
            self.data.devices.append(Device(id, outputs[device_name], self))
            self.data.mailboxes.append(pygame.midi.Input(id))
            self.data.preprocessors.append([])
            if device_name == 'MPD218':
                self.data.preprocessors[-1].append(MPD218Preprocessor())

        print('----- current wiring -----')
        for device in self.data.devices:
            print(device)
        print('--------------------------')

    def set_scripts(self, message):
        self.scriptsCache = ScriptsCache(self, message)

    def set_buttons(self, message):
        self.data.buttons = [None] + [ControlButton(x.replace('%20', ' ')) for x in message.split('|')]

    def enable_metronome(self):
        self.socket_send('metronome-on-event')
        self.is_metronome_on = True

    def disable_metronome(self):
        self.socket_send('metronome-off-event')
        self.is_metronome_on = False

    def metronome_tick_on(self, n):
        self.socket_send(f'metronome-tick {n}')
        index = self.data.beats - n
        print(index) # countdown
        if self.is_metronome_on:
            note = Note(0, 36, 120)
            if index <= 4 and self.song_started == False:
                note = Note(0, 37, 120)
            self.metronome_output.note_on(note)

    def metronome_tick_off(self):
        note = Note(0, 36, 120)
        self.metronome_output.note_off(note)

    def start(self):
        if len(self.data.devices) == 0:
            print(f'{bcolors.WARNING}Server has 0 configured devices!{bcolors.ENDC}')
            return
        if self.state != server_states.pending:
            print(f'{bcolors.WARNING}Server is not ready!{bcolors.ENDC}')
            return
        if self.scriptsCache == None:
            print(f'{bcolors.WARNING}Scripts are not set!{bcolors.ENDC}')
            return
        if self.data.loop_length == None:
            print(f'{bcolors.WARNING}Loop length is not set!{bcolors.ENDC}')
            return
        if self.data.beats == None:
            print(f'{bcolors.WARNING}Beats number is not set!{bcolors.ENDC}')
            return
        if self.data.buttons == None:
            print(f'{bcolors.WARNING}Buttons are not set!{bcolors.ENDC}')
            return
        if self.metronome_output == None:
            print(f'{bcolors.WARNING}Devices are not set!{bcolors.ENDC}')
            return

        self.data.start_time = time.time() * 1000 + (self.data.loop_length / self.data.beats) * 0.3
        self.state = server_states.run
        self.scriptsCache.execute_immediately(0)
        self.socket_send(f'sync {self.data.start_time}')

    def stop(self):
        self.state = server_states.pending
        for device in self.data.devices:
            device.stop()
        time.sleep(0.5)
        hard_reset_midi()

    def process(self):
        self._last_current_time = self._current_time
        self._current_time = (time.time() * 1000 - self.data.start_time) % self.data.loop_length

        if self._last_current_time == 0:
            self._last_current_time = self._current_time
                
        for i, device in enumerate(self.data.devices):
            midi_events = []
            
            if self.data.mailboxes[i].poll():
                midi_events = self.data.mailboxes[i].read(10)
                for pp in self.data.preprocessors[i]:
                    midi_events = pp.process_input(midi_events)
                for event in midi_events:
                    is_pressed = event[0][0] == 144 or event[0][0] == 153
                    if is_pressed:
                        for button_index, button in enumerate(self.data.buttons):
                            if button_index == 0:
                                continue
                            if button.check_event(self.data.mailboxes[i], event):
                                print(f'{bcolors.OKCYAN}STAGED SCRIPT "{self.scriptsCache.get_script_source(button_index).replace(".", " ").replace(">", ", ")}"{bcolors.ENDC}')
                                self.scriptsCache.stage_script(button_index)
            
            device.process(self._current_time, midi_events)
            
        if self._current_time < self._last_current_time:
            print('loop!')
            self.loops_count += 1
            for device in self.data.devices:
                device.end_loop()
            self.scriptsCache.fire_staged_scripts()
            if (self.scriptsCache.fired_count > 0):
                self.song_started = True
            for device in self.data.devices:
                device.start_loop()

        beat_length = self.data.loop_length / self.data.beats
        last_beat = floor(self._last_current_time / beat_length)
        current_beat = floor(self._current_time / beat_length)
        last_beat_progress = (self._last_current_time / beat_length - last_beat)
        current_beat_progress = (self._current_time / beat_length - last_beat)
        last_subbeat = floor(last_beat_progress * 4)
        current_subbeat = floor(current_beat_progress * 4)
        if last_beat != current_beat:
            self.metronome_tick_on(current_beat)
        if last_subbeat == 0 and current_subbeat == 1:
            self.metronome_tick_off()
        

    def find_tape(self, index):
        name = f'loopMIDI Port {index}'
        for device in self.data.devices:
            for tape in device.tapes:
                if tape.port_index == index:
                    return tape
        raise Exception(f'No tape with name "{name}"')

    def socket_send(self, message):
        self._socket_send_generator.send(message)

    def _create_socket_send_generator(self):
        delay = 100
        delay_remains = delay
        queue = Queue()
        while True:
            delay_remains -= 1
            if delay_remains <= 0:
                delay_remains = delay
                if not queue.empty():
                    self.client_socket.send(bytes(queue.get(), 'utf-8'))
            message = yield
            if message:
                queue.put(message)            

    def stop_server(self):
        try:
            self.client_socket.send(b'stop')
            self.client_socket.shutdown(socket.SHUT_RDWR)
            self.client_socket.close()
            print(f"{bcolors.HEADER}Server closed{bcolors.ENDC}")
        except:
            pass
        finally:
            exit()

    def __enter__(self):
        return self

    def __exit__(self, exc_type, exc_val, exc_tb):
        self.stop_server()
        exit()


#-- utils --

name_to_deviceinfo_map = None

def current_time():
    return round(time.time() * 1000)

def hard_reset_midi():
    pygame.midi.quit()
    time.sleep(0.5)
    pygame.midi.init()
    time.sleep(0.5)

def get_input_device_name(id):
    info = pygame.midi.get_device_info(id)
    device_count = pygame.midi.get_count()
    all_devices = [pygame.midi.get_device_info(i) for i in range(device_count)]
    same_devices = [(i, device) for i, device in enumerate(all_devices) if device[2] > 0 and device[1] == info[1]]

    if len(same_devices) <= 1:
        return info[1].decode()
    else:
        index = 0
        letters = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H']
        for i, device in same_devices:
            if i == id:
                return f'{info[1].decode()} [{letters[index]}]'
            if info[1] == device[1] and info[2] == device[2]:
                index += 1
        raise "error while getting input device name"

def get_input_device_id(name):
    global name_to_deviceinfo_map
    if not name_to_deviceinfo_map:

        name_to_deviceinfo_map = {}
        letters = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H']
        device_count = pygame.midi.get_count()
        all_devices = [pygame.midi.get_device_info(i) for i in range(device_count)]
        same_devices_dict = {}

        for i, device in enumerate(all_devices):
            if device[2] == 0:
                continue
            devicename = device[1].decode()
            if 'loopMIDI' in devicename:
                continue
            if devicename in same_devices_dict:
                same_devices_dict[devicename] += 1
            else: 
                same_devices_dict[devicename] = 0
                name_to_deviceinfo_map[devicename] = i
            letter = letters[same_devices_dict[devicename]]
            name_to_deviceinfo_map[f'{devicename} [{letter}]'] = i
        
    return name_to_deviceinfo_map[name]

'''
port - int number 1 or larger
'''
def get_output_device_id(port):
    device_count = pygame.midi.get_count()
    all_devices = [pygame.midi.get_device_info(i) for i in range(device_count)]
    output_devices_names = [x[1].decode() if x[3] > 0 else '' for x in all_devices]
    name = f'loopMIDI Port {port}'
    if name in output_devices_names:
        return output_devices_names.index(name)
    else:
        return None

if __name__ == "__main__":
    pygame.midi.init()
    with Server('localhost', 12345) as server:
        #server.set_devices(ports)
        server.accept()
        while True:
            try:
                server.mainloop()
            except KeyboardInterrupt:
                print('Closed by keyboard interruption')
                break
            except Exception as err:
                print(err)
                traceback.print_exc()
                exit()
                break