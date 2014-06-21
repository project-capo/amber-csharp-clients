using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using LeapMotionPandaSteering.Common;
using Amber_API.Amber;
using Amber_API.Drivers;


namespace LeapMotionPandaSteering.Listeners
{
    public class RoboclawListener : Listener
    {
        private Object thisLock = new Object();
        private FrameState previousFrameState;
        private Vector zeroVector;

        public RoboclawProxy Proxy { get; private set; } 

        public RoboclawListener(RoboclawProxy proxy)
        {
            this.Proxy = proxy;
        }

        public RoboclawListener() { }

        private void SafeWriteLine(String line)
        {
            lock (thisLock)
            {
                Console.WriteLine(line);
            }
        }

        public override void OnInit(Controller controller)
        {
            previousFrameState = new FrameState();
            SafeWriteLine("Initialized");
        }

        public override void OnConnect(Controller controller)
        {
            SafeWriteLine("Connected");
            controller.EnableGesture(Gesture.GestureType.TYPE_CIRCLE);
            controller.EnableGesture(Gesture.GestureType.TYPE_KEY_TAP);
            controller.EnableGesture(Gesture.GestureType.TYPE_SCREEN_TAP);
            controller.EnableGesture(Gesture.GestureType.TYPE_SWIPE);
        }

        public override void OnDisconnect(Controller controller)
        {
            SafeWriteLine("Disconnected");
        }

        public override void OnExit(Controller controller)
        {
            SafeWriteLine("Exited");
        }

        public override void OnFrame(Controller controller)
        {
            Frame frame = controller.Frame();
                        
            ControlHandEvents(frame);         
        }

        private void ControlHandEvents(Frame frame)
        {
            if (frame.Hands.Count == 2 && frame.Gestures()[0].Type == Gesture.GestureType.TYPE_CIRCLE)
            {
                var circle = new CircleGesture(frame.Gestures()[0]);
                if (circle.Pointable.Direction.AngleTo(circle.Normal) <= Math.PI/4)
                {
                    MotionInterpreter.Circle(Proxy, 1);
                }
                else
                {
                    MotionInterpreter.Circle(Proxy, -1);
                }
            }

            if (frame.Hands.Count == 0)
            {
                MotionInterpreter.Stop(Proxy);
            }
            if (frame.Hands.Count == 1 && frame.Hands[0].GrabStrength == 1.0)
            {                
                zeroVector = null;
                MotionInterpreter.Stop(Proxy);
            }

            if (frame.Hands.Count == 1 && frame.Hands[0].GrabStrength < 1.0)
            {
                if (zeroVector == null && frame.IsValid)
                {
                    zeroVector = frame.Hands[0].PalmPosition;
                    return;
                }

                var palm = frame.Hands[0].PalmPosition;
                if (MotionInterpreter.ComputeRoboclawSpeed(Proxy, palm, zeroVector) == false)
                {
                    SafeWriteLine("Could not set roboclaw speed");
                    return;
                }
            }

            if (frame.Hands.Count == 0 && zeroVector != null)
            {                
                zeroVector = null;
                MotionInterpreter.Stop(Proxy);
            }
            
        }
        private Direction DetermineDirection(SwipeGesture gesture)
        {
            var direction = gesture.Direction;
            if (gesture.Direction.x < 0 && Math.Abs(direction.x) > Math.Abs(direction.z))
                return Direction.West;
            if (gesture.Direction.x >= 0 && Math.Abs(direction.x) > Math.Abs(direction.z))
                return Direction.East;
            if (gesture.Direction.z < 0 && Math.Abs(direction.z) > Math.Abs(direction.x))
                return Direction.North;
            if (gesture.Direction.z >= 0 && Math.Abs(direction.z) > Math.Abs(direction.x))
                return Direction.South;
            return Direction.South;
        }
    }

    public enum Direction
    {
        North, South, East, West, Up, Down
    }
}
