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
        private IAmberListener<NinedofOutcome> _ninedofOutcomeListener;
        private readonly Dictionary<uint, NinedofOutcome> _awaitingOutcomes;
        private readonly Object _listenerLock = new object();

        public NinedofProxy(AmberClient amberClient, int deviceID)
            : base((int) Drivers.DeviceType.Ninedof, deviceID, amberClient)
        {
            Debug.WriteLine("Starting and registering NinedofProxy.");
            _extensionRegistry = ExtensionRegistry.CreateInstance();
            _awaitingOutcomes = new Dictionary<uint, NinedofOutcome>();
            Ninedof.RegisterAllExtensions(_extensionRegistry);
        }

        public void RegisterNinedofOutcomeListener(uint freq, Boolean accel, Boolean gyro, Boolean magnet, IAmberListener<NinedofOutcome> listener)
        {
            DriverMsg driverMsg = buildSubscribeActionMsg(freq, accel, gyro, magnet);

            lock (_listenerLock)
            {
                _ninedofOutcomeListener = listener;
            }

            AmberClient.SendMessage(BuildHeader(), driverMsg);
        }

        public void UnregisterNinedofOutcomeListener()
        {
            DriverMsg driverMsg = buildSubscribeActionMsg(0, false, false, false);

            lock (this)
            {
                _ninedofOutcomeListener = null;
            }

            AmberClient.SendMessage(BuildHeader(), driverMsg);
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
            if (!message.HasAckNum || ackNum == 0)
            {
                var outcome = new NinedofOutcome();
                SensorData sensorData = message.GetExtension(Ninedof.SensorData);

                FillStructure(outcome, sensorData);

                outcome.Available = true;

                lock (_listenerLock)
                {
                    if (_ninedofOutcomeListener != null)
                    {
                        _ninedofOutcomeListener.Handle(outcome);
                    }
                }
            } else {
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

        private DriverMsg buildSubscribeActionMsg(uint freq, bool accel, bool gyro, bool magnet)
        {
            SubscribeAction.Builder subscribeActionBuilder = SubscribeAction.CreateBuilder();

            subscribeActionBuilder.Freq = freq;
            subscribeActionBuilder.Accel = accel;            
            subscribeActionBuilder.Gyro = gyro;
            subscribeActionBuilder.Magnet = magnet;

            DriverMsg.Builder driverMsgBuilder = DriverMsg.CreateBuilder();
            driverMsgBuilder.Type = DriverMsg.Types.MsgType.DATA;
            driverMsgBuilder.SetExtension(Ninedof.SubscribeAction, subscribeActionBuilder.Build());

            return driverMsgBuilder.Build();
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
