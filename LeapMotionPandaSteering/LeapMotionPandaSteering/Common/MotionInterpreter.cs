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
        public static bool ComputeRoboclawSpeed(RoboclawProxy proxy, Vector palmPosition, Vector zeroVector)
        {
            return false;
        }

        public static void Stop(RoboclawProxy proxy)
        {
            proxy.SetSpeed(0, 0, 0, 0);
        }
    }
}
