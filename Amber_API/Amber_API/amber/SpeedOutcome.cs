using System;
using System.Threading;

namespace Amber_API.Amber
{
    public class SpeedOutcome
    {
        private bool _available;
        protected bool Available
        {
            get
            {
                if (Exception != null)
                {
                    throw Exception;
                }
                return _available;
            }
            set
            {
                lock (this)
                {
                    _available = value;
                    autoEvent.Set();
                }       
            }
        }

        private int _frontLeftSpeed;
        public int FrontLeftSpeed
        {
            get
            {
                if (!Available)
                    WaitAvailable();
                return _frontLeftSpeed;
            }
            set { _frontLeftSpeed = value;  }
        }

        private int _frontRightSpeed;
        public int FrontRightSpeed
        {
            get
            {
                if (!Available)
                    WaitAvailable();
                return _frontRightSpeed;
            }
            set { _frontRightSpeed = value;  }
        }

        private int _rearLeftSpeed;
        public int RearLeftSpeed
        {
            get
            {
                if (!Available)
                    WaitAvailable();
                return _rearLeftSpeed;
            }
            set { _rearLeftSpeed = value;  }
        }

        private int _rearRightSpeed;
        public int RearRightSpeed
        {
            get
            {
                if (!Available)
                    WaitAvailable();
                return _rearRightSpeed;
            }
            set { _rearRightSpeed = value;  }
        }

        public  Exception Exception { get; set; }
        private AutoResetEvent autoEvent = new AutoResetEvent(false);

        public void WaitAvailable()
        {
            lock (this)
            {
                while (Available)
                {
                    autoEvent.WaitOne();
                }
            }    
        }
    }
}
