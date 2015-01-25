using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amber_API.Amber
{
    public class NinedofDataListener : IAmberListener<NinedofOutcome>
    {
        public void Handle(NinedofOutcome data)
        {
            Console.WriteLine(
                   "Ninedof current parameters: gyro:\n\tX: {0}\n\tY: {1}\n\tZ: {2}\naccel:\n\tX: {3}\n\tY: {4}\n\tZ: {5}",
                   data.Gyro.XAxis, data.Gyro.YAxis, data.Gyro.ZAxis,
                   data.Accel.XAxis, data.Accel.YAxis, data.Accel.ZAxis);
        }
    }
}
