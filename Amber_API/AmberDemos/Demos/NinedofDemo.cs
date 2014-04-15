using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Amber.Protos;
using Amber_API.Amber;
using Amber_API.Drivers;

namespace AmberDemos.Demos
{
    public class NinedofDemo
    {
        public void RunDemo()
        {
            AmberClient client = AmberClient.Create("192.168.1.50", 26233);

            var ninedofProxy = new NinedofProxy(client, 1);

            for (int i = 0; i < 10; i++)
            {
                var ninedofOutcome = ninedofProxy.GetAxesData(true, true, false);
                ninedofOutcome.WaitAvailable();

                Console.WriteLine(
                    "Ninedof current parameters: gyro:\n\tX: {0}\n\tY: {1}\n\tZ: {2}\naccel:\n\tX: {3}\n\tY: {4}\n\tZ: {5}",
                    ninedofOutcome.Gyro.XAxis, ninedofOutcome.Gyro.YAxis, ninedofOutcome.Gyro.ZAxis,
                    ninedofOutcome.Accel.XAxis, ninedofOutcome.Accel.YAxis, ninedofOutcome.Accel.ZAxis);
                Thread.Sleep(10);
            }

            Console.WriteLine("Now registering cyclic data listener.");
            Thread.Sleep(1000);
            
            ninedofProxy.RegisterNinedofOutcomeListener(10, true, true, false, new NinedofDataListener());
            Thread.Sleep(100000);
        }


    }
}
