using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amber_API.Drivers;
using Leap;
using LeapMotionPandaSteering.Listeners;

namespace LeapMotionPandaSteering.Common
{
    public static class MotionInterpreter
    {
        public static int MaxSpeed = 500;
        public static int MaxAmplitude = 300;

        public static bool ComputeRoboclawSpeed(RoboclawProxy proxy, Vector palmPosition, Vector zeroVector)
        {
            var diffrentialPosition = new Vector(palmPosition.x - zeroVector.x,palmPosition.y - zeroVector.y,zeroVector.z - palmPosition.z);
            if(Math.Abs(diffrentialPosition.z) < 100 && Math.Abs(diffrentialPosition.x)<100){
                return true;
            }
            int forwardingSpeed = 0;
            int baseTurningSpeed = 0;
            if(diffrentialPosition.x < 0)
                baseTurningSpeed = (MaxSpeed * (int)diffrentialPosition.x+100) / MaxAmplitude;
            else
                baseTurningSpeed = (MaxSpeed * (int)diffrentialPosition.x - 100) / MaxAmplitude;
            if(diffrentialPosition.z < 0)
                forwardingSpeed = (MaxSpeed * ((int)diffrentialPosition.z+100)) / MaxAmplitude;
            else
                forwardingSpeed = (MaxSpeed * ((int)diffrentialPosition.z-100)) / MaxAmplitude;

            var fl = MotionInterpreter.ComputeFrontLeftSpeed(baseTurningSpeed, forwardingSpeed);
            var rl = MotionInterpreter.ComputeRearLeftSpeed(baseTurningSpeed, forwardingSpeed);
            var fr = MotionInterpreter.ComputeFrontRightSpeed(baseTurningSpeed, forwardingSpeed);
            var rr = MotionInterpreter.ComputeRearRightSpeed(baseTurningSpeed, forwardingSpeed);

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

        public static bool RunSwipeRoboclaw(RoboclawProxy proxy, Direction direction)
        {
            try
            {
                if(direction == Direction.West){
                    proxy.SetSpeed(800,-200,800,-200);
                } else if(direction == Direction.East){
                    proxy.SetSpeed(-200, 800, -200, 800);
                }
                System.Threading.Thread.Sleep(1500);
                Stop(proxy);
            }

            catch (Exception e)
            {
                Console.WriteLine("Could not set roboclaw speed", e);
                return false;
            }

            return true;
        }

        public static void Stop(RoboclawProxy proxy)
        {
            proxy.SetSpeed(0, 0, 0, 0);
        }

        private static int ComputeRearLeftSpeed(int baseTurningSpeed, int forwardingSpeed)
        {
            //if (forwardingSpeed < 0)
              //  return 0;

            if (baseTurningSpeed <= 0)
                return forwardingSpeed;

            if (forwardingSpeed < 0)
                return -baseTurningSpeed + forwardingSpeed;
            return baseTurningSpeed + forwardingSpeed;
        }

        private static int ComputeRearRightSpeed(int baseTurningSpeed, int forwardingSpeed)
        {
            //if (forwardingSpeed < 0)
              //  return 0;

            if (baseTurningSpeed > 0)
                return forwardingSpeed;

            if (forwardingSpeed < 0)
                return baseTurningSpeed + forwardingSpeed;
            return Math.Abs(baseTurningSpeed) + forwardingSpeed;
        }

        private static int ComputeFrontLeftSpeed(int baseTurningSpeed, int forwardingSpeed)
        {
            //if (forwardingSpeed < 0)
              //  return 0;

            if (baseTurningSpeed <= 0)
                return forwardingSpeed;

            if (forwardingSpeed < 0)
                return -baseTurningSpeed + forwardingSpeed;
            return baseTurningSpeed + forwardingSpeed;
        }

        private static int ComputeFrontRightSpeed(int baseTurningSpeed, int forwardingSpeed)
        {
            
              //  return 0;

            if (baseTurningSpeed > 0)
                return forwardingSpeed;
            if (forwardingSpeed < 0)
                return baseTurningSpeed + forwardingSpeed;
            return Math.Abs(baseTurningSpeed) + forwardingSpeed;
        }




    }
}
