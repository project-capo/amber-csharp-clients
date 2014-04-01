using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using amber;
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
