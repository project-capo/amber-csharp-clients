using System;
using System.Diagnostics;
using amber;
using amber.roboclaw_proto;
using Amber_API.Amber;
using Google.ProtocolBuffers;

namespace Amber_API.Drivers
{
    public class RoboclawProxy : AmberProxy
    {
        private DeviceType Type = Drivers.DeviceType.Roboclaw;
        private readonly ExtensionRegistry _extensionRegistry;

        public RoboclawProxy(AmberClient amberClient, int deviceID) 
            : base ((int)Drivers.DeviceType.Roboclaw,deviceID,amberClient)
        {
            Debug.WriteLine("Starting and registering RoboclawProxy.");
            _extensionRegistry = ExtensionRegistry.CreateInstance();
            Roboclaw.RegisterAllExtensions(_extensionRegistry);
        }

        public override void HandleDataMsg(DriverHdr header, DriverMsg message)
        {
            //todo: finish method 
            var ackNum = message.AckNum;
		    if (message.HasAckNum && ackNum != 0)
            {
            
			} else {
			
		

	        }
        }

        public override ExtensionRegistry GetExtensionRegistry()
        {
            return _extensionRegistry;
        }
    }

    public enum DeviceType
    {
        Roboclaw = 2,
        Stargazer = 3
    }
}
