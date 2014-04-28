using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Leap;

namespace LeapMotionPandaSteering.Common
{
    public class FrameState
    {
        public int HandsCount { get; set; }
        public int FingerCount { get; set; }
        public Vector HandInitialPosition { get; set; }

        public FrameState()
        {
            HandsCount = 0;
            FingerCount = 0;
            HandInitialPosition = Vector.Zero;
        }
    }
}
