using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Amber_API.Amber
{
    public class ConnectionHandler
    {
        public int _port;
        public Socket Socket { get; set; }
        public IPAddress IpAddress { get; set; }
        public UdpClient UdpClient { get; set; }
        public IPEndPoint SendEndPoint;
        public IPEndPoint ReceiveEndPoint;

        public ConnectionHandler(string hostname, int port)
        {
            _port = port;
            IpAddress = IPAddress.Parse(hostname);
            UdpClient = new UdpClient(port);            
            SendEndPoint = new IPEndPoint(IpAddress, port);
            ReceiveEndPoint = new IPEndPoint(IPAddress.Any, port);
        }

        public void Send(byte[] bytes)
        {
            UdpClient.Send(bytes, bytes.Length, SendEndPoint);
            //Socket.SendTo(bytes, bytes.Length, SocketFlags.None, SendEndPoint);  
        }

        public void Terminate()
        {
        }
    }
}
