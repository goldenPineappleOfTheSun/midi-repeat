1. install loopMIDI
2. create a bunch of ports named like: f'loopMIDI Port {port}' where port is the number from from 1 to 12
3. also create a port named 'loopMIDI Port metronome'. These ports will serve as virtual players
4. install pygame.midi
5. create a file with the name f'{song_name}.midiconf'. Put in the folder where the exe file is
6. optionally also put a '{song_name}.wav' file in the same folder 
7. make sure the last version of server.py file is present there
8. run any daw, disable real input devices and then connect loopMIDI ones
9. ...
10. PROFIT
