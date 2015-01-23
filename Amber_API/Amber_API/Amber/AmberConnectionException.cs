using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amber_API.Amber
{
    public class AmberConnectionException : Exception
    {
        public AmberConnectionException(string message, Exception innerException) 
            : base(message, innerException)
        {
            
        }
        private static readonly long serialVersionUID = -7736321114486909539L;
    }
}
