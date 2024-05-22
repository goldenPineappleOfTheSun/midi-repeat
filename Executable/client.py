# Client code
import socket
import re

def create():
    SERVER_ADDRESS = 'localhost'
    SERVER_PORT = 12345
    client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
    client_socket.connect((SERVER_ADDRESS, SERVER_PORT))
    return client_socket

def send(client_socket, text):
    text = re.sub(r'\[[\w\d ,\/]+\]', lambda x: x.group()[1:-1].replace(', ', '>').replace(',', '>').replace(' ', '.'), text)
    client_socket.sendall(text.encode())
    if text == 'stop':
        client_socket.close()