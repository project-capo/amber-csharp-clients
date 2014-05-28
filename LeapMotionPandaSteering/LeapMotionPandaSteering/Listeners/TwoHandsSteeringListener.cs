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
        private Vector zeroVector;
        private Vector initialLeftHandPosition;
        private Vector initialRightHandPosition;
        private SwipeGesture previousGesture;

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
            controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
            controller.EnableGesture(Gesture.GestureType.TYPEKEYTAP);
            controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
            controller.EnableGesture(Gesture.GestureType.TYPESWIPE);
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
                initialLeftHandPosition = frame.Hands[0].PalmPosition;
                initialRightHandPosition = frame.Hands[1].PalmPosition;
            }
            else if (frame.Hands.Count == 2 && frame.Hands[0].Fingers.Count > 0 && initialLeftHandPosition != null &&
                     frame.Hands[1].Fingers.Count > 0 && initialRightHandPosition != null)
            {
                SafeWriteLine("steering");
                TwoHandsMotionInterpreter.ComputeRoboclawSpeed(Proxy, frame.Hands[0].PalmPosition,
                    initialLeftHandPosition, frame.Hands[1].PalmPosition, initialRightHandPosition);
            }
            else if (frame.Hands.Count == 0)
            {
                SafeWriteLine("reset");
                initialLeftHandPosition = null;
                initialRightHandPosition = null;
            }
            else
            {
                SafeWriteLine("stopping");
                TwoHandsMotionInterpreter.Stop(Proxy);
            }
            previousFrameState.HandsCount = frame.Hands.Count;
        }
        

        


    }
}
