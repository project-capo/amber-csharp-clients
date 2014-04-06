using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amber_API.Amber
{
    public class NinedofOutcome : Outcome
    {
        private AxesData _accel;        

        public AxesData Accel
        {
            get 
            {
                if (!Available)
                {
                    WaitAvailable();
                }
                return _accel; 
            }
            set { _accel = value; }
        }

        private AxesData _gyro;

        public AxesData Gyro
        {
            get
            {
                if (!Available)
                {
                    WaitAvailable();
                }
                return _gyro;
            }
            set { _gyro = value; }
        }

        private AxesData _magnet;

        public AxesData Magnet
        {
            get
            {
                if (!Available)
                {
                    WaitAvailable();
                }
                return _magnet;
            }
            set { _magnet = value; }
        }
    }
}
