using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using Amber.Protos;
using Google.ProtocolBuffers;

namespace Amber_API.Amber
{
    public class AmberClient
    {
        public int Port
        {
            get { return this.ConnectionHandler._port; }         
        }

        private ConnectionHandler ConnectionHandler { get; set; }

        private readonly int buffSize = 4096;
        private bool isActive;

        private List<AmberProxy> proxies = new List<AmberProxy>();
        private Thread receivingThread;

        private AmberClient(string hostname, int port)
        {
            InitializeConnection(hostname, port);
            isActive = true;
            receivingThread = new Thread(Run);
            receivingThread.Start();
        }

        private void InitializeConnection(string hostname, int port)
        {
            try
            {
                ConnectionHandler = new ConnectionHandler(hostname, port);
            }
            catch (SocketException e)
            {
                throw new AmberConnectionException("Socket Error occured", e);
            }
        }

        public static AmberClient Create(string hostname, int port)
        {
            return new AmberClient(hostname, port);
        }

        public void RegisterClient(AmberProxy proxy)
        {
            proxies.Add(proxy);
        }

        public void Terminate()
        {
            if (!isActive)
            {
                return;
            }

            foreach (AmberProxy proxy in proxies) 
            {
			    proxy.TerminateProxy();
		    }

            ConnectionHandler.Terminate();
            isActive = false;
            receivingThread.Abort();
            
        }

        private void Run()
        {
            MessageReceivingLoop();
        }

        private void MessageReceivingLoop()
        {
            byte[] packet = new byte[buffSize];            
            AmberProxy clientProxy = null;
            while (isActive)
            {
                try
                {
                    packet = ConnectionHandler.UdpClient.Receive(ref ConnectionHandler.ReceiveEndPoint);
                    int headerLen = (packet[0] << 8 | packet[1]);
                    ByteString headerByteString = ByteString.CopyFrom(packet, 2, headerLen);
                    DriverHdr header = DriverHdr.ParseFrom(headerByteString);
                    int messageLen = (packet[2 + headerLen] << 8) | packet[2 + headerLen + 1];
                    ByteString messageByteString = ByteString.CopyFrom(packet, 2 + headerLen + 2, messageLen);
               
                    if (!header.HasDeviceType || !header.HasDeviceID || header.DeviceType == 0)
                    {
                        var message = DriverMsg.ParseFrom(messageByteString);
                        MessageHandler.HandleMessageFromMediator(header, message);
                    }
                    else
                    {
                        clientProxy = proxies.Single(s => s.DeviceId == header.DeviceID && s.DeviceType == header.DeviceType);
                        if (clientProxy == null)
                        {
                            Debug.WriteLine("Client proxy with given device type {0} and ID {1} not found. Ignoring message.", header.DeviceType, header.DeviceID);
                            continue;
                        }
                        var message = DriverMsg.ParseFrom(messageByteString, clientProxy.GetExtensionRegistry());
                        MessageHandler.HandleMessageFromDriver(header, message, clientProxy);
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
            //var k = header.ToString();
            MemoryStream memoryStream = new MemoryStream();

            int length;            
            

            length = header.SerializedSize;
            byte[] lengthHeaderBytes = new byte[2];
            lengthHeaderBytes[0] = (byte) (length >> 8);
            lengthHeaderBytes[1] = (byte) (length);
            //BitConverter.GetBytes(length);
            memoryStream.Write(lengthHeaderBytes, 0, lengthHeaderBytes.Length);

            byte[] headerBytes = header.ToByteArray();
            memoryStream.Write(headerBytes, 0, headerBytes.Length);

            var g = memoryStream.ToArray();

            length = message.SerializedSize;
            byte[] lengthMessageBytes = new byte[2];
            lengthMessageBytes[0] = (byte)(length >> 8);
            lengthMessageBytes[1] = (byte)(length);
            memoryStream.Write(lengthMessageBytes, 0, lengthMessageBytes.Length);

            byte[] messageBytes = message.ToByteArray();
            memoryStream.Write(messageBytes, 0, messageBytes.Length);

            byte[] toSendBytes = memoryStream.ToArray();
            Send(toSendBytes);
        }

        public void Send(byte [] array)
        {
            ConnectionHandler.Send(array);
            //Roboclaw.Motor
        }

       
    }
}
