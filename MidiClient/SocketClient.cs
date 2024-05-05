using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MidiClient
{
    public class SocketClient : IDisposable
    {
        public delegate void MessageDelegate(string message);
        public event MessageDelegate MessageReceived;

        private TcpClient client;
        private NetworkStream stream;
        private bool isConnected;
        private Thread receiveThread;

        private void RunServer()
        {
            var fullPath = System.Reflection.Assembly.GetEntryAssembly().Location;
            var path = fullPath.Substring(0, fullPath.Length - 14);

            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    Arguments = path + "server.py",
                    FileName = "Python"
                }
            };
            process.Start();
            Thread.Sleep(300);
        }

        public SocketClient()
        {
            isConnected = false;
        }

        public bool CheckConnection()
        {
            if (!isConnected)
            {
                return false;
            }
            try
            {
                IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
                TcpConnectionInformation[] tcpConnections = ipProperties.GetActiveTcpConnections().Where(x => x.LocalEndPoint.Equals(GetLocalEndPoint()) && x.RemoteEndPoint.Equals(GetRemoteEndPoint())).ToArray();
                isConnected = tcpConnections.First().State == TcpState.Established;
                return isConnected;
            } 
            catch
            {
                return false;
            }
        }

        public bool Connect(string serverAddress, int port)
        {
            if (!CheckConnection())
            {
                try
                {
                    RunServer();
                    client = new TcpClient(serverAddress, port);
                    stream = client.GetStream();
                    isConnected = true;

                    // Start receiving messages in a separate thread
                    receiveThread = new Thread(Receive);
                    receiveThread.Start();

                    return true;
                }
                catch (Exception ex)
                {
                    // Handle connection error
                    Console.WriteLine("Error connecting to server: " + ex.Message);
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Already connected to server.");
                return true;
            }
        }

        public void Close()
        {
            if (CheckConnection())
            {
                stream.Close();
                client.Close();
                isConnected = false;
                Console.WriteLine("Connection closed.");

                // Abort receive thread
                receiveThread.Abort();
            }
        }

        public void Send(string message)
        {
            if (CheckConnection())
            {
                byte[] buffer = Encoding.ASCII.GetBytes(message);
                stream.Write(buffer, 0, buffer.Length);
                stream.Flush();
                Console.WriteLine("Message sent: " + message);
            }
            else
            {
                Console.WriteLine("Not connected to server. Cannot send message.");
            }
        }

        public EndPoint GetLocalEndPoint()
        {
            return client.Client.LocalEndPoint;
        }

        public EndPoint GetRemoteEndPoint()
        {
            return client.Client.RemoteEndPoint;
        }

        private void Receive()
        {
            try
            {
                while (isConnected)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    if (message != "")
                    {
                        MessageReceived(message);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error receiving message: " + ex.Message);
            }
        }

        public void Dispose()
        {
            Close();
        }
    }
}
