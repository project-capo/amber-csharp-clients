using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Amber_API.amber
{
    public interface IAmberListener<T>
    {
        void Handle(T data);
    }
}
