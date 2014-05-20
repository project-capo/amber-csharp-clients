using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amber_API.Drivers;
using Leap;

namespace LeapMotionPandaSteering.Common
{
    public static class MotionInterpreter
    {
        public static int MaxSpeed = 300;
        public static int MaxAmplitude = 500;

        public static bool ComputeRoboclawSpeed(RoboclawProxy proxy, Vector palmPosition, Vector zeroVector)
        {
            var diffrentialPosition = new Vector(palmPosition.x - zeroVector.x,palmPosition.y - zeroVector.y,zeroVector.z - palmPosition.z);
            var baseTurningSpeed = (MaxSpeed * (int)diffrentialPosition.x) / MaxAmplitude;
            var forwardingSpeed = (MaxSpeed * (int)diffrentialPosition.z) / MaxAmplitude;

            var fl = ComputeFrontLeftSpeed(baseTurningSpeed, forwardingSpeed);
            var rl = ComputeRearLeftSpeed(baseTurningSpeed, forwardingSpeed);
            var fr = ComputeFrontRightSpeed(baseTurningSpeed, forwardingSpeed);
            var rr = ComputeRearRightSpeed(baseTurningSpeed, forwardingSpeed);

            try
            {
                proxy.SetSpeed(fl, fr, rl, rr);
            }

            catch (Exception e)
            {
                Console.WriteLine("Could not set roboclaw speed", e);
                return false;
            }

            return true;
            //Console.WriteLine(fl + " " + fr + " " + rl + " " + rr);
        }

        public static void Stop(RoboclawProxy proxy)
        {
            proxy.SetSpeed(0, 0, 0, 0);
        }

        private static int ComputeRearLeftSpeed(int baseTurningSpeed, int forwardingSpeed)
        {
            if (forwardingSpeed < 0)
                return 0;

            if (baseTurningSpeed <= 0)
                return forwardingSpeed;

            return baseTurningSpeed + forwardingSpeed;
        }

        private static int ComputeRearRightSpeed(int baseTurningSpeed, int forwardingSpeed)
        {
            if (forwardingSpeed < 0)
                return 0;

            if (baseTurningSpeed > 0)
                return forwardingSpeed;

            return Math.Abs(baseTurningSpeed) + forwardingSpeed;
        }

        private static int ComputeFrontLeftSpeed(int baseTurningSpeed, int forwardingSpeed)
        {
            if (forwardingSpeed < 0 || baseTurningSpeed <= 0)
                return 0;

            return baseTurningSpeed;
        }

        private static int ComputeFrontRightSpeed(int baseTurningSpeed, int forwardingSpeed)
        {
            if (forwardingSpeed < 0 || baseTurningSpeed > 0)
                return 0;

            return Math.Abs(baseTurningSpeed);
        }




    }
}
