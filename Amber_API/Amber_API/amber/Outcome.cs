using System;
using System.Threading;

namespace Amber_API.Amber
{
    public abstract class Outcome
    {
        private bool _available;
        public bool Available
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

        public Exception Exception { get; set; }
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
