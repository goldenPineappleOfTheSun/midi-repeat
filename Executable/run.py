import client
import time
import random

socket = client.create()
time.sleep(1)
client.send(socket, f'set-basics 14784 32')
time.sleep(0.5)
client.send(socket, f'set-wiring MPK%20mini%203|MPD218|MPD218|MPD218')
time.sleep(0.5)
client.send(socket, f'set-scripts monitor.1>monitor.2>metronome.on/record.1>record.2/play.1>play.2>record.3>metronome.on/play.3/monitor.4/monitor.1>mute.2>mute.3>monitor.4/play.1>play.2/record.1>mute.2>mute.3>monitor.4/play.1>play.2>play.3>monitor.4')
time.sleep(0.5)
client.send(socket, f'set-buttons MPD218%20event-153%20note-44|MPD218%20event-153%20note-45|MPD218%20event-153%20note-46|MPD218%20event-153%20note-47|MPD218%20event-153%20note-48|MPD218%20event-153%20note-49|MPD218%20event-153%20note-50|MPK%20mini%203%20[A]%20event-153%20note-43')
time.sleep(0.5)
client.send(socket, f'start')

input()
client.send(socket, f'stop')
