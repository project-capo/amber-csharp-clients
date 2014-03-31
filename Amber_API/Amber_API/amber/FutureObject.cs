using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Amber_API.amber
{
    public class FutureObject
    {
        protected bool available = false;

        protected Exception exception = null;
        private AutoResetEvent autoEvent = new AutoResetEvent(false);

        public bool isAvailable()
        {
            if (exception != null)
            {
                throw exception;
            }
            return available;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void waitAvailable()
        {
            try
            {
                while (!isAvailable())
                {
                    autoEvent.WaitOne();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void setAvailable()
        {
            available = true;
            autoEvent.Set();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void setException(Exception e)
        {
            available = true;
            exception = e;
            autoEvent.Set();
        }
    }
}
