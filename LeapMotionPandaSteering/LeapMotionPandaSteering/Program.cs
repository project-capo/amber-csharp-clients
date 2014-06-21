using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using LeapMotionPandaSteering.Listeners;
using Amber_API.Drivers;
using Amber_API.Amber;
using System.Xml.Serialization;
using System.IO;

namespace LeapMotionPandaSteering
{
    public class Program
    {
        static void Main(string[] args)
        {
            Settings settings = InitializeConfiguration("config.xml");
            if (settings == null)
                return;

            SetMotionInterpreterParameters(settings);
            AmberClient client = null;
            try
            {
                client = AmberClient.Create(settings.ClientIP, settings.ClientPort);
                var roboclawProxy = new RoboclawProxy(client, 0);
                var controller = new Controller(); 
                
                Listener listener;

                switch (settings.SteeringModeHandsCount)
                {
                    case 1:
                        listener = new RoboclawListener(roboclawProxy);
                        controller.AddListener(listener);
                        break;
                    case 2:
                        listener = new TwoHandsSteeringListener(roboclawProxy);
                        controller.AddListener(listener);
                        break;
                    default:
                        listener = new TwoHandsSteeringListener(roboclawProxy);
                        controller.AddListener(listener);
                        break;
                }

                Console.ReadKey();

                controller.RemoveListener(listener);
                controller.Dispose();
            }

            finally
            {
                if(client!=null)
                    client.Terminate();
            }
           
        }

        private static void SetMotionInterpreterParameters(Settings settings)
        {
            if(settings.Delay!=null)
                LeapMotionPandaSteering.Common.MotionInterpreter.DelayMs = (int) settings.Delay;
            if (settings.CircleSpeed != null)
                LeapMotionPandaSteering.Common.MotionInterpreter.CircleSpeed = (int)settings.CircleSpeed;
            if (settings.MaxAmplitude != null)
                LeapMotionPandaSteering.Common.MotionInterpreter.MaxAmplitude = (int)settings.MaxAmplitude;
            if (settings.MaxSpeed != null)
                LeapMotionPandaSteering.Common.MotionInterpreter.MaxSpeed = (int)settings.MaxSpeed;
            if (settings.PeaceArea != null)
                LeapMotionPandaSteering.Common.MotionInterpreter.PeaceArea = (int)settings.PeaceArea;
        }

        private static Settings InitializeConfiguration(string path)
        {
            try
            {
                LeapMotionPandaSteering.Common.MotionInterpreter.Initialize();
                var serializer = new XmlSerializer(typeof(Settings));

                using (var reader = new StreamReader(path))
                {
                    return (Settings)serializer.Deserialize(reader);

                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Could not load configuration. Ensure config.xml content is valid.", e);
                return null;
            }
        }
    }

    [Serializable()]
    [XmlRoot("Settings")]
    public class Settings
    {
        [XmlElement]
        public string ClientIP { get; set; }

        [XmlElement]
        public int ClientPort { get; set; }

        [XmlElement(IsNullable = true)]
        public int? Delay { get; set; }

        [XmlElement(IsNullable = true)]
        public int? MaxSpeed { get; set; }

        [XmlElement(IsNullable = true)]
        public int? MaxAmplitude { get; set; }

        [XmlElement(IsNullable = true)]
        public int? PeaceArea { get; set; }

        [XmlElement(IsNullable = true)]
        public int? CircleSpeed { get; set; }

        [XmlElement]
        public int SteeringModeHandsCount { get; set; }
    }
 
}
