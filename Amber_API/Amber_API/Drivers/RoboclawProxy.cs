using System;
using System.Collections.Generic;
using System.Diagnostics;
using Amber.Protos;
using Amber_API.Amber;
using Google.ProtocolBuffers;

namespace Amber_API.Drivers
{
    public class RoboclawProxy : AmberProxy
    {
        private readonly ExtensionRegistry _extensionRegistry;
        private readonly Dictionary<uint, SpeedOutcome> _awaitingOutcomes;

        public RoboclawProxy(AmberClient amberClient, int deviceID) 
            : base ((int)Drivers.DeviceType.Roboclaw,deviceID,amberClient)
        {
            Debug.WriteLine("Starting and registering RoboclawProxy.");
            _extensionRegistry = ExtensionRegistry.CreateInstance();
            _awaitingOutcomes = new Dictionary<uint, SpeedOutcome>();
            Roboclaw.RegisterAllExtensions(_extensionRegistry);
        }

        public void SetSpeed(int frontLeftSpeed, int frontRightSpeed, int rearLeftSpeed, int rearRightSpeed)
        {
            DriverMsg.Builder driverMsgBuilder = DriverMsg.CreateBuilder();
            driverMsgBuilder.Type = DriverMsg.Types.MsgType.DATA;
            
            MotorsSpeed.Builder commandBuilder = MotorsSpeed.CreateBuilder();

            commandBuilder.FrontLeftSpeed = frontLeftSpeed;
            commandBuilder.FrontRightSpeed = frontRightSpeed;
            commandBuilder.RearLeftSpeed = rearLeftSpeed;
            commandBuilder.RearRightSpeed = rearRightSpeed;

            MotorsSpeed motorsSpeed = commandBuilder.Build();
            driverMsgBuilder.SetExtension(Roboclaw.MotorsCommand, motorsSpeed);

            driverMsgBuilder.SynNum = NextSyncNumber;
            
            AmberClient.SendMessage(BuildHeader(), driverMsgBuilder.Build());
        }

        public SpeedOutcome GetSpeed()
        {

            DriverMsg.Builder driverMsgBuilder = DriverMsg.CreateBuilder();
		    driverMsgBuilder.Type = DriverMsg.Types.MsgType.DATA;

            driverMsgBuilder.SetExtension(Roboclaw.CurrentSpeedRequest, true);

            driverMsgBuilder.SynNum = NextSyncNumber;

            DriverMsg currentSpeedRequestMsg = driverMsgBuilder.Build();

	        var outcome = new SpeedOutcome();
	        _awaitingOutcomes.Add(driverMsgBuilder.SynNum, outcome);

	        AmberClient.SendMessage(BuildHeader(), currentSpeedRequestMsg);

	        return outcome;
        }

        public override void HandleDataMsg(DriverHdr header, DriverMsg message)
        {
            var ackNum = message.AckNum;
            if (message.HasAckNum && ackNum != 0)
            {
                
                if (_awaitingOutcomes.ContainsKey(ackNum))
                {
                    SpeedOutcome outcome = _awaitingOutcomes[ackNum];
                    MotorsSpeed inputMcs = message.GetExtension(Roboclaw.CurrentSpeed);

                    outcome.FrontLeftSpeed = inputMcs.FrontLeftSpeed;
                    outcome.FrontRightSpeed = inputMcs.FrontRightSpeed;
                    outcome.RearLeftSpeed = inputMcs.RearLeftSpeed;
                    outcome.RearRightSpeed = inputMcs.RearRightSpeed;
                    outcome.Available = true;
                    _awaitingOutcomes.Remove(ackNum);
                }

            }
        }

        public override ExtensionRegistry GetExtensionRegistry()
        {
            return _extensionRegistry;
        }
    }

    public enum DeviceType
    {
        Ninedof = 1,
        Roboclaw = 2,
        Stargazer = 3
    }
}
