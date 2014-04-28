using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using LeapMotionPandaSteering.Listeners;

namespace LeapMotionPandaSteering
{
    public class Program
    {
        static void Main(string[] args)
        {
            var listener = new RoboclawListener();
            var controller = new Controller();
            listener.RegisterOnOneHandAppearListener(OnHandAppear);

            controller.AddListener(listener);

            Console.ReadLine();

            controller.RemoveListener(listener);
            controller.Dispose();
        }

        private static void OnHandAppear()
        {
            Console.WriteLine("Tu handler");
        }
    }

 
}
