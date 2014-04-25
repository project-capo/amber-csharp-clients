using System.Diagnostics;
using System.IO;
using Amber.Protos;
using Google.ProtocolBuffers;

namespace Amber_API.Amber
{
    public abstract class AmberProxy
    {
        protected AmberProxy(){}

        public AmberClient AmberClient { get; private set; }
        public int DeviceType { get; private set; }
        public int DeviceId { get; private set; }

        public AmberProxy(int deviceType, int deviceId, AmberClient amberClient)
        {            
            DeviceId = deviceId;
            DeviceType = deviceType;
            AmberClient = amberClient;

            amberClient.RegisterClient(this);
        }

        private static uint _syncnumber;
        public static uint NextSyncNumber
        {
            get { return ++_syncnumber; }
        }

        public abstract void HandleDataMsg(DriverHdr header, DriverMsg message);

        public void HandlePingMsg(DriverHdr header, DriverMsg message)
        {
        }

        public void HandlePongMsg(DriverHdr header, DriverMsg message)
        {
        }

        public void HandleDriverDiedMsg(DriverHdr header, DriverMsg message)
        {
        }

        public abstract ExtensionRegistry GetExtensionRegistry();

        protected DriverHdr BuildHeader()
        {
            DriverHdr.Builder driverHdrBuilder = DriverHdr.CreateBuilder();
            driverHdrBuilder.SetDeviceType(DeviceType);
            driverHdrBuilder.SetDeviceID(DeviceId);
            //driverHdrBuilder
            //driverHdrBuilder.SetDeviceType(DeviceType);
            //driverHdrBuilder.SetDeviceID(DeviceId);

            return driverHdrBuilder.Build();
        }

        public void TerminateProxy()
        {
            DriverMsg.Builder driverMsgBuilder = DriverMsg.CreateBuilder();
            driverMsgBuilder.SetType(DriverMsg.Types.MsgType.CLIENT_DIED);

            try
            {
                AmberClient.SendMessage(BuildHeader(), driverMsgBuilder.Build());
            }

            catch (IOException e)
            {
                Debug.WriteLine("Error in sending terminate message");
            }
        }
    }
}
