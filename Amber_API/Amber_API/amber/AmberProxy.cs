using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using amber;
using Google.ProtocolBuffers;

namespace Amber_API.amber
{
    public abstract class AmberProxy
    {
        protected readonly AmberClient amberClient;
        protected readonly int deviceType;
        protected readonly int deviceID;

        public AmberProxy(int deviceType, int deviceID, AmberClient amberClient)
        {
            this.deviceType = deviceType;
            this.deviceID = deviceID;
            this.amberClient = amberClient;

            amberClient.RegisterClient(deviceType, deviceID, this);
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
            driverHdrBuilder.SetDeviceType(deviceType);
            driverHdrBuilder.SetDeviceID(deviceID);

            return driverHdrBuilder.Build();
        }

    }
}
