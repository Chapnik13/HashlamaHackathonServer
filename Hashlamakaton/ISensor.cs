using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hashlamakaton
{
    interface ISensor
    {
        bool IsAvilable { get; }
        object GetCurrent();
    }
}
