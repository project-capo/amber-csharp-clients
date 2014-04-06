using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amber_API.Amber
{
    public class AxesData
    {
        public int XAxis { get; set; }
        public int YAxis { get; set; }
        public int ZAxis { get; set; }

        public AxesData(int xAxis, int yAxis, int zAxis)
        {
            XAxis = xAxis;
            YAxis = yAxis;
            ZAxis = zAxis;
        }
    }
}
