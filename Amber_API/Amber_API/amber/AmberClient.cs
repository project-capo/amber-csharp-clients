using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using amber;
using Google.ProtocolBuffers;

namespace Amber_API.amber
{
    public class AmberClient
    {
        private readonly Socket socket;
        private readonly UdpClient udpClient;
        private readonly IPAddress address;
        private readonly int port;
        private IPEndPoint sendEndPoint;
        private IPEndPoint receiveEndPoint;

        private readonly int buffSize = 512;
        private static readonly int RECEIVING_TIMEOUT = 1000;

        private Dictionary<Tuple<int, int>, AmberProxy> proxyDictionary;
        private Thread receivingThread;

        public void RegisterClient(int deviceType, int deviceID, AmberProxy proxy)
        {
            proxyDictionary.Add(new Tuple<int, int>(deviceType, deviceID), proxy);
        }

        public AmberClient(string hostname, int port)
        {
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);                
                address = IPAddress.Parse(hostname);
                udpClient = new UdpClient(0);
                this.port = port;
                sendEndPoint = new IPEndPoint(address, port); 
             
                receiveEndPoint = new IPEndPoint(IPAddress.Any, port);

                receivingThread = new Thread(Run);
            }
            catch (SocketException e)
            {
                throw new AmberConnectionException();
            }
        }

        private void Run()
        {
            MessageReceivingLoop();
        }

        private void MessageReceivingLoop()
        {
            byte[] packet = new byte[buffSize];            
            AmberProxy clientProxy = null;
            while (true)
            {
                try
                {
                    packet = udpClient.Receive(ref receiveEndPoint);
                    int headerLen = (packet[0] << 8 | packet[1]);
                    ByteString headerByteString = ByteString.CopyFrom(packet, 2, headerLen);
                    DriverHdr header = DriverHdr.ParseFrom(headerByteString);

                    int messageLen = (packet[2 + headerLen] << 8) | packet[2 + headerLen + 1];
                    ByteString messageByteString = ByteString.CopyFrom(packet, 2 + headerLen + 2, messageLen);
                    DriverMsg message;
                    if (!header.HasDeviceType || !header.HasDeviceID || header.DeviceID == 0)
                    {
                        message = DriverMsg.ParseFrom(messageByteString);
                    }
                    else
                    {
                        proxyDictionary.TryGetValue(new Tuple<int, int>(header.DeviceType, header.DeviceID),
                            out clientProxy);
                        if (clientProxy == null)
                        {
                            Debug.WriteLine("Client proxy with given device type {0} and ID {1} not found. Ignoring message.", header.DeviceType, header.DeviceID);
                            continue;
                        }
                        message = DriverMsg.ParseFrom(messageByteString, clientProxy.GetExtensionRegistry());
                    }

                    proxyDictionary.TryGetValue(new Tuple<int, int>(header.DeviceType, header.DeviceID), out clientProxy);
                    if (clientProxy == null)
                    {
                        Debug.WriteLine("Client proxy with given device type {0} and ID {1} not found. Ignoring message.", header.DeviceType, header.DeviceID);
                        continue;
                    }

                    message = DriverMsg.ParseFrom(messageByteString, clientProxy.GetExtensionRegistry());

                    switch (message.Type)
                    {
                        case DriverMsg.Types.MsgType.DATA:
                            clientProxy.HandleDataMsg(header, message);
                            break;
                        case DriverMsg.Types.MsgType.PING:
                            clientProxy.HandlePingMsg(header, message);
                            break;
                        case DriverMsg.Types.MsgType.PONG:
                            clientProxy.HandlePongMsg(header, message);
                            break;
                        case DriverMsg.Types.MsgType.DRIVER_DIED:
                            clientProxy.HandleDriverDiedMsg(header, message);
                            break;
                        default:
                            continue;
                    }
                }
                catch (InvalidProtocolBufferException ex)
                {
                    Debug.WriteLine("Cannot deserialize the message");
                }
                catch (IOException e)
                {
                    Debug.WriteLine("IOException while receiving packet");
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void SendMessage(DriverHdr header, DriverMsg message)
        {
            MemoryStream memoryStream = new MemoryStream();

            int length;            
            

            length = header.SerializedSize;
            byte[] lengthHeaderBytes = BitConverter.GetBytes(length);
            memoryStream.Write(lengthHeaderBytes, 0, lengthHeaderBytes.Length);

            byte[] headerBytes = header.ToByteArray();
            memoryStream.Write(headerBytes, 0, headerBytes.Length);

            length = message.SerializedSize;
            byte[] lengthMessageBytes = BitConverter.GetBytes(length);
            memoryStream.Write(lengthMessageBytes, 0, lengthMessageBytes.Length);

            byte[] messageBytes = message.ToByteArray();
            memoryStream.Write(messageBytes, 0, messageBytes.Length);

            byte[] toSendBytes = memoryStream.ToArray();
            socket.SendTo(toSendBytes, toSendBytes.Length, SocketFlags.None, sendEndPoint);
        }
    }
}
