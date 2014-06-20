using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amber_API.Drivers;
using Leap;
using LeapMotionPandaSteering.Listeners;
using System.Diagnostics;

namespace LeapMotionPandaSteering.Common
{
    public static class MotionInterpreter
    {
        public static int MaxSpeed = 1000;
        public static int MaxAmplitude = 300;
        public static int PeaceArea = 100;
        public static Stopwatch TimeSinceLastMessage;
        public static int DelayMs = 100;

        public static void Initialize()
        {
            TimeSinceLastMessage = new Stopwatch();
            TimeSinceLastMessage.Start();
        }

        public static bool ComputeRoboclawSpeed(RoboclawProxy proxy, Vector palmPosition, Vector zeroVector, bool peaceAreaEnabled=true)
        {
            if (TimeSinceLastMessage.ElapsedMilliseconds < MotionInterpreter.DelayMs)
                return true;

            var diffrentialPosition = new Vector(palmPosition.x - zeroVector.x,palmPosition.y - zeroVector.y,zeroVector.z - palmPosition.z);
            if (peaceAreaEnabled && Math.Abs(diffrentialPosition.z) < PeaceArea && Math.Abs(diffrentialPosition.x) < PeaceArea)
            {
                return true;
            }

            int forwardingSpeed = 0;
            int baseTurningSpeed = 0;
            if(diffrentialPosition.x < 0)
                baseTurningSpeed = (MaxSpeed * ((int)diffrentialPosition.x + PeaceArea)) / MaxAmplitude;
            else
                baseTurningSpeed = (MaxSpeed * ((int)diffrentialPosition.x - PeaceArea)) / MaxAmplitude;
            if(diffrentialPosition.z < 0)
                forwardingSpeed = (MaxSpeed * ((int)diffrentialPosition.z + PeaceArea)) / MaxAmplitude;
            else
                forwardingSpeed = (MaxSpeed * ((int)diffrentialPosition.z - PeaceArea)) / MaxAmplitude;

            var fl = ComputeLeftSpeedOneHand(baseTurningSpeed, forwardingSpeed);
            var rl = ComputeLeftSpeedOneHand(baseTurningSpeed, forwardingSpeed);
            var fr = ComputeRightSpeedOneHand(baseTurningSpeed, forwardingSpeed);
            var rr = ComputeRightSpeedOneHand(baseTurningSpeed, forwardingSpeed);

            try
            {
                proxy.SetSpeed(fl, fr, rl, rr);
                TimeSinceLastMessage.Restart();
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

        private static int ComputeLeftSpeedOneHand(int baseTurningSpeed, int forwardingSpeed)
        {
            if (baseTurningSpeed <= 0)
                return forwardingSpeed;

            if (forwardingSpeed < 0)
                return -baseTurningSpeed + forwardingSpeed;
            return baseTurningSpeed + forwardingSpeed;
        }

        private static int ComputeRightSpeedOneHand(int baseTurningSpeed, int forwardingSpeed)
        {
            if (baseTurningSpeed > 0)
                return forwardingSpeed;

            if (forwardingSpeed < 0)
                return baseTurningSpeed + forwardingSpeed;
            return Math.Abs(baseTurningSpeed) + forwardingSpeed;
        }

        public static bool ComputeRoboclawSpeed(RoboclawProxy proxy, Vector currentLeftHandPosition,
            Vector initialLeftHandPosition, Vector currentRightHandPosition, Vector initialRightHandPosition)
        {
            var forwardingSpeed = MaxSpeed * ((int)currentRightHandPosition.z - (int)initialRightHandPosition.z) / MaxAmplitude;
            var turningSpeed = MaxSpeed * ((int)currentLeftHandPosition.x - (int)initialLeftHandPosition.x) / MaxAmplitude;
            var fl = ComputeLeftSpeedTwoHands(forwardingSpeed, turningSpeed);
            var fr = ComputeRightSpeedTwoHands(forwardingSpeed, turningSpeed);
            var rl = ComputeLeftSpeedTwoHands(forwardingSpeed, turningSpeed);
            var rr = ComputeRightSpeedTwoHands(forwardingSpeed, turningSpeed);

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

        public static void Circle(RoboclawProxy proxy, int direction)
        {
            int circleSpeed = 200;
            try
            {
                proxy.SetSpeed(circleSpeed*direction, circleSpeed*direction, -circleSpeed*direction, -circleSpeed*direction);
            }
            catch (Exception e)
            {
                Console.WriteLine("Could not set roboclaw speed", e);                
            }
        }

        private static int ComputeRightSpeedTwoHands(int forwardingSpeed, int turningSpeed)
        {
            return (forwardingSpeed < 0) ?
                -(forwardingSpeed + turningSpeed) : -(forwardingSpeed - turningSpeed);
        }

        private static int ComputeLeftSpeedTwoHands(int forwardingSpeed, int turningSpeed)
        {
            return (forwardingSpeed < 0) ?
                -(forwardingSpeed - turningSpeed) : -(forwardingSpeed + turningSpeed);
        }        
    }
}
