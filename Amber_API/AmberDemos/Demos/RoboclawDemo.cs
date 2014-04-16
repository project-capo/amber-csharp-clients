using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amber.Protos;
using Amber_API.Amber;
using Amber_API.Drivers;

namespace AmberDemos.Demos
{
    public class RoboclawDemo
    {
        public void RunDemo()
        {
            AmberClient client = AmberClient.Create("192.168.1.50", 26233);

            var roboclawProxy = new RoboclawProxy(client, 0);

            int speed = 1000;

            roboclawProxy.SetSpeed(speed, speed, speed, speed);
            var speedOutcome = roboclawProxy.GetSpeed();
            speedOutcome.WaitAvailable();

            Console.WriteLine("Motors current speed: fl: {0}, fr: {1}, rl: {2}, rr: {3}",
                speedOutcome.FrontLeftSpeed, speedOutcome.FrontRightSpeed, speedOutcome.RearLeftSpeed, speedOutcome.RearRightSpeed);
        }
    }
}
