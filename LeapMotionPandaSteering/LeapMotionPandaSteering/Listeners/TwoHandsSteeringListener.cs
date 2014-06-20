using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amber_API.Drivers;
using Leap;
using LeapMotionPandaSteering.Common;

namespace LeapMotionPandaSteering.Listeners
{
    public class TwoHandsSteeringListener : Listener
    {
        private Object thisLock = new Object();
        private FrameState previousFrameState;
        private Vector initialLeftHandPosition;
        private Vector initialRightHandPosition;

        public RoboclawProxy Proxy { get; private set; } 

        public HandAppearDelegate OnOneHandAppear;
        public HandDisappearDelegate OnHandDisappear;

        public TwoHandsSteeringListener(RoboclawProxy proxy)
        {
            Proxy = proxy;
        }

        public TwoHandsSteeringListener() { }

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
            if (frame.Hands.Count == 2 && frame.Hands[0].Fingers.Count > 0 && initialLeftHandPosition == null && frame.Hands[1].Fingers.Count > 0 && initialRightHandPosition == null && previousFrameState.HandsCount < 2)
            {
                SafeWriteLine("init");
                if (frame.Hands[0].PalmPosition.x < frame.Hands[1].PalmPosition.x)
                {
                    initialLeftHandPosition = frame.Hands[0].PalmPosition;
                    initialRightHandPosition = frame.Hands[1].PalmPosition;
                }
                else
                {
                    initialRightHandPosition = frame.Hands[0].PalmPosition;
                    initialLeftHandPosition = frame.Hands[1].PalmPosition;
                }
            }
            else if (frame.Hands.Count == 2 && frame.Hands[0].Fingers.Count > 0 && initialLeftHandPosition != null &&
                     frame.Hands[1].Fingers.Count > 0 && initialRightHandPosition != null)
            {
//                SafeWriteLine("steering");
                //if (frame.Id % 9 != 0)
                //    return;
                Vector left;
                Vector right;
                if (frame.Hands[0].PalmPosition.x < frame.Hands[1].PalmPosition.x)
                {
                    left = frame.Hands[0].PalmPosition;
                    right = frame.Hands[1].PalmPosition;
                }
                else
                {
                    right = frame.Hands[0].PalmPosition;
                    left = frame.Hands[1].PalmPosition;
                }

                MotionInterpreter.ComputeRoboclawSpeed(Proxy, left,
                    initialLeftHandPosition, right, initialRightHandPosition);
            }
            else if (frame.Hands.Count == 1 && frame.Gestures()[0].Type == Gesture.GestureType.TYPE_CIRCLE)
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
            else if (frame.Hands.Count == 0)
            {
//                SafeWriteLine("reset");
                initialLeftHandPosition = null;
                initialRightHandPosition = null;
                MotionInterpreter.Stop(Proxy);
            }
            /*else if(frame.Id % 3 == 0)
            {
//                SafeWriteLine("stopping");
                MotionInterpreter.Stop(Proxy);
            }*/
            previousFrameState.HandsCount = frame.Hands.Count;
        }
        

        


    }
}
