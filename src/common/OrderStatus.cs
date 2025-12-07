using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common
{
    public enum OrderStatus
    {
        PENDING,
        FULLY_FILLED,
        PARTIAL_FILLED,
        PARTIAL_CANCELLED,
        FULL_CANCELLED,
    }
}
