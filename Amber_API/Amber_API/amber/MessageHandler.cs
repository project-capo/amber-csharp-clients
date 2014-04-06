using System.Diagnostics;
using Amber.Protos;

namespace Amber_API.Amber
{
    public static class MessageHandler
    {
        public static void HandleMessageFromDriver(DriverHdr header, DriverMsg message, AmberProxy clientProxy)
        {
            switch (message.Type)
            {
                case DriverMsg.Types.MsgType.DATA:
                    Debug.WriteLine("DATA message came for {0}:{1}, handling.", clientProxy.DeviceType, clientProxy.DeviceId);
                    clientProxy.HandleDataMsg(header, message);
                    break;
                case DriverMsg.Types.MsgType.PING:
                    Debug.WriteLine("PING message came for {0}:{1}, handling.", clientProxy.DeviceType, clientProxy.DeviceId);
                    clientProxy.HandlePingMsg(header, message);
                    break;
                case DriverMsg.Types.MsgType.PONG:
                    Debug.WriteLine("PONG message came for {0}:{1}, handling.", clientProxy.DeviceType, clientProxy.DeviceId);
                    clientProxy.HandlePongMsg(header, message);
                    break;
                case DriverMsg.Types.MsgType.DRIVER_DIED:
                    Debug.WriteLine("DRIVER_DIED message came for {0}:{1}, handling.", clientProxy.DeviceType, clientProxy.DeviceId);
                    clientProxy.HandleDriverDiedMsg(header, message);
                    break;
                default:
                    Debug.WriteLine("Unexpected message came {0} for {1}:{2}, ignoring", message.Type, clientProxy.DeviceType, clientProxy.DeviceId);
                    break;
            }
        }

        public static void HandleMessageFromMediator(DriverHdr header, DriverMsg message)
        {
            switch (message.Type)
            {
                case DriverMsg.Types.MsgType.DATA:
                    Debug.WriteLine("DATA message came, but device details not set, ignoring.");
                    break;
                case DriverMsg.Types.MsgType.PING:
                    Debug.WriteLine("PING message came, handling.");
                    HandlePingMsg(header, message);
                    break;
                case DriverMsg.Types.MsgType.PONG:
                    Debug.WriteLine("PONG message came, handling.");
                    HandlePongMsg(header, message);
                    break;
                case DriverMsg.Types.MsgType.DRIVER_DIED:
                    Debug.WriteLine("DATA message came, but device details not set, ignoring.");
                    break;
                default:
                    Debug.WriteLine("Unexpected message came {0}, ignoring", message.Type);
                    break;
            }
        }

        public static void HandlePongMsg(DriverHdr header, DriverMsg message)
        {
        }

        public static void HandlePingMsg(DriverHdr header, DriverMsg message)
        {
        }
    }
}
