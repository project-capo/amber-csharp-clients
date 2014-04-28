using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;
using LeapMotionPandaSteering.Common;

public delegate void HandAppearDelegate();
public delegate void HandDisappearDelegate();

namespace LeapMotionPandaSteering.Listeners
{
    public class RoboclawListener : Listener
    {
        private Object thisLock = new Object();
        private FrameState previousFrameState;

        public HandAppearDelegate OnOneHandAppear;
        public HandDisappearDelegate OnHandDisappear;
        


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
                        
            ControlHandEvents(frame.Hands.Count);
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

        private void ControlHandEvents(int handsCount)
        {
            if (previousFrameState.HandsCount == 0 && handsCount > previousFrameState.HandsCount && handsCount == 1)
            {
                SafeWriteLine("Tu 1 rąsia");
                if (OnOneHandAppear != null)
                {
                    OnOneHandAppear();
                }
            }
            else if (previousFrameState.HandsCount == 1 && handsCount > previousFrameState.HandsCount)
            {
               SafeWriteLine("Tu 2 rąsia"); 
            }
            else if (previousFrameState.HandsCount == 2 && handsCount < previousFrameState.HandsCount && handsCount == 1)
            {
               SafeWriteLine("Jedna rąsia żegna"); 
            }
            else if (handsCount < previousFrameState.HandsCount && handsCount == 0)
            {
                SafeWriteLine("Nie ma rąk");
                if (OnHandDisappear != null)
                {
                    OnHandDisappear();
                }
            }
        }
    }
}
