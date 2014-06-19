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
            AmberClient client = AmberClient.Create(settings.ClientIP, settings.ClientPort);
            try
            {
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
                client.Terminate();
            }
           
        }

        private static Settings InitializeConfiguration(string path)
        {
            LeapMotionPandaSteering.Common.MotionInterpreter.Initialize();
            var serializer = new XmlSerializer(typeof(Settings));

            using (var reader = new StreamReader(path))
            {
                return (Settings)serializer.Deserialize(reader);
                
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

        [XmlElement]
        public int SteeringModeHandsCount { get; set; }
    }
 
}
