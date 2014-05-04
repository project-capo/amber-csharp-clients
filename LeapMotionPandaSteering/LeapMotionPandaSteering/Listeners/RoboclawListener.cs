using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using LeapMotionPandaSteering.Common;
using Amber_API.Amber;
using Amber_API.Drivers;

public delegate void HandAppearDelegate();
public delegate void HandDisappearDelegate();

namespace LeapMotionPandaSteering.Listeners
{
    public class RoboclawListener : Listener
    {
        private Object thisLock = new Object();
        private FrameState previousFrameState;
        private Vector zeroVector;

        public RoboclawProxy Proxy { get; private set; } 

        public HandAppearDelegate OnOneHandAppear;
        public HandDisappearDelegate OnHandDisappear;

        public RoboclawListener(RoboclawProxy proxy)
        {
            this.Proxy = proxy;
        }

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
            SetPreviousFrameState(frame.Hands.Count);            
        }

        private void SetPreviousFrameState(int handsCount)
        {
            previousFrameState.HandsCount = handsCount;
        }

        public void RegisterOnOneHandAppearListener(HandAppearDelegate listener)
        {
            OnOneHandAppear += listener;
        }

        private void ControlHandEvents(Frame frame)
        {
            if (frame.Hands.Count == 1 && frame.Fingers.Count == 0)
            {
                SafeWriteLine("Zero vector reset");
                zeroVector = null;
                MotionInterpreter.Stop(Proxy);
            }

            if (frame.Hands.Count == 1 && frame.Fingers.Count > 0)
            {
                if (zeroVector == null && frame.IsValid)
                {
                    zeroVector = frame.Hands[0].PalmPosition;
                    return;
                }

                //anti-flood
                if(frame.Id % 10 != 0)
                    return;

                var palm = frame.Hands[0].PalmPosition;
                if (MotionInterpreter.ComputeRoboclawSpeed(Proxy, palm, zeroVector) == false)
                {
                    SafeWriteLine("Could not set roboclaw speed");
                    return;
                }

            }

            if (frame.Hands.Count == 0 && zeroVector != null)
            {
                SafeWriteLine("Zero vector reset");
                zeroVector = null;
                MotionInterpreter.Stop(Proxy);
            }
        }
    }
}
