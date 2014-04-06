using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amber.Protos;
using Amber_API.Amber;
using Google.ProtocolBuffers;

namespace Amber_API.Drivers
{
    public class NinedofProxy : AmberProxy
    {
        private readonly ExtensionRegistry _extensionRegistry;
        private readonly Dictionary<uint, NinedofOutcome> _awaitingOutcomes;

        public NinedofProxy(AmberClient amberClient, int deviceID)
            : base((int) Drivers.DeviceType.Ninedof, deviceID, amberClient)
        {
            Debug.WriteLine("Starting and registering NinedofProxy.");
            _extensionRegistry = ExtensionRegistry.CreateInstance();
            _awaitingOutcomes = new Dictionary<uint, NinedofOutcome>();
            Ninedof.RegisterAllExtensions(_extensionRegistry);
        }
        
        public NinedofOutcome GetAxesData(Boolean accel, Boolean gyro, Boolean magnet)
        {            
            Debug.WriteLine("Pulling Ninedof data.");
            var driverMessage = BuildDataRequestMsg(accel, gyro, magnet);

            var outcome = new NinedofOutcome();
            _awaitingOutcomes.Add(driverMessage.SynNum, outcome);

            AmberClient.SendMessage(BuildHeader(), driverMessage);
            return outcome;
        }

        public override void HandleDataMsg(DriverHdr header, DriverMsg message)
        {
            var ackNum = message.AckNum;
            if (message.HasAckNum && ackNum != 0)
            {
                if (_awaitingOutcomes.ContainsKey(ackNum))
                {
                    NinedofOutcome outcome = _awaitingOutcomes[ackNum];
                    SensorData sensorData = message.GetExtension(Ninedof.SensorData);

                    FillStructure(outcome, sensorData);

                    outcome.Available = true;
                    _awaitingOutcomes.Remove(ackNum);
                }
            }
        }

        public override ExtensionRegistry GetExtensionRegistry()
        {
            return _extensionRegistry;
        }

        private static DriverMsg BuildDataRequestMsg(bool accel, bool gyro, bool magnet)
        {
            DataRequest.Builder dataRequestBuilder = DataRequest.CreateBuilder();
            dataRequestBuilder.Accel = accel;
            dataRequestBuilder.Gyro = gyro;
            dataRequestBuilder.Magnet = magnet;

            DriverMsg.Builder driverMessageBuilder = DriverMsg.CreateBuilder();            
            driverMessageBuilder.SynNum = NextSyncNumber;
            driverMessageBuilder.SetExtension(Ninedof.DataRequest, dataRequestBuilder.Build());
            driverMessageBuilder.Type = DriverMsg.Types.MsgType.DATA;

            return driverMessageBuilder.Build();
        }        

        private static void FillStructure(NinedofOutcome outcome, SensorData sensorData)
        {
            outcome.Accel = new AxesData(sensorData.Accel.XAxis, sensorData.Accel.YAxis, sensorData.Accel.ZAxis);
            outcome.Gyro = new AxesData(sensorData.Gyro.XAxis, sensorData.Gyro.YAxis, sensorData.Gyro.ZAxis);
            outcome.Magnet = new AxesData(sensorData.Magnet.XAxis, sensorData.Magnet.YAxis, sensorData.Magnet.ZAxis);
        }

        
    }
}
