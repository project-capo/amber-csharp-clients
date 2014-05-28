using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amber_API.Drivers;
using Leap;

namespace LeapMotionPandaSteering.Common
{
    public static class TwoHandsMotionInterpreter
    {
        private static int maxSpeed = 1000;
        private static int maxAmplitude = 300;

        public static bool ComputeRoboclawSpeed(RoboclawProxy proxy, Vector currentLeftHandPosition,
            Vector initialLeftHandPosition, Vector currentRightHandPosition, Vector initialRightHandPosition)
        {
            var forwardingSpeed = maxSpeed * ((int)currentRightHandPosition.z - (int)initialRightHandPosition.z) / maxAmplitude;
            var turningSpeed = maxSpeed * ((int)currentLeftHandPosition.x - (int)initialLeftHandPosition.x) / maxAmplitude;
            var fl = ComputeLeftSpeed(forwardingSpeed, turningSpeed);
            var fr = ComputeRightSpeed(forwardingSpeed, turningSpeed);
            var rl = ComputeLeftSpeed(forwardingSpeed, turningSpeed);
            var rr = ComputeRightSpeed(forwardingSpeed, turningSpeed);
            
            try
            {
                proxy.SetSpeed(fl, fr, rl, rr);
            }

            catch (Exception e)
            {
                Console.WriteLine("Could not set roboclaw speed", e);
                return false;
            }

            Console.WriteLine(fl + " " + fr + " " + rl + " " + rr);
            return true;
        }

        public static void Stop(RoboclawProxy proxy)
        {
            proxy.SetSpeed(0, 0, 0, 0);
        }

        private static int ComputeRightSpeed(int forwardingSpeed, int turningSpeed)
        {
            return (forwardingSpeed < 0) ?
                -(forwardingSpeed + turningSpeed) : -(forwardingSpeed - turningSpeed);           
        }

        private static int ComputeLeftSpeed(int forwardingSpeed, int turningSpeed)
        {
            return (forwardingSpeed < 0) ?
                -(forwardingSpeed - turningSpeed) : -(forwardingSpeed + turningSpeed);
        }        
    }
}
