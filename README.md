1. install loopMIDI
2. create bunch of ports named like: f'loopMIDI Port {port}' where port is the number from from 1 to 12
3. also create a port named 'loopMIDI Port metronome'. They will serve as virtual players
4. install pygame.midi
5. create a file with the name f'{song_name}.midiconf'. Put in the folder where exe file is
6. optionally also put in the same folder a '{song_name}.wav' file
7. make sure the last version of server.py file is present there
8. run some daw, disable real input devices and connect loopMIDI ones
9. ...
10. PROFIT
