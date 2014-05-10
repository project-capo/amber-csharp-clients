using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using LeapMotionPandaSteering.Listeners;
using Amber_API.Drivers;
using Amber_API.Amber;

namespace LeapMotionPandaSteering
{
    public class Program
    {
        static void Main(string[] args)
        {
            AmberClient client = AmberClient.Create("192.168.2.205", 26233);
            try
            {
                var roboclawProxy = new RoboclawProxy(client, 0);

                //int speed = 1000;

                //roboclawProxy.SetSpeed(speed, speed, speed, speed);

                var listener = new RoboclawListener(roboclawProxy);

                var controller = new Controller();
                //listener.RegisterOnOneHandAppearListener(OnHandAppear);

                controller.AddListener(listener);

                Console.ReadKey();

                controller.RemoveListener(listener);
                controller.Dispose();
            }

            finally
            {
                client.Terminate();
            }

           
        }
        /*
        private static void OnHandAppear()
        {
            Console.WriteLine("Tu handler");
        }*/
    }

 
}
