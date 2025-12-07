using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace common
{
    public class OrderEntity
    {
        public long id { get; set; }
        public long sequenceId { get; set; }
        public long userId { get; set; }

        public decimal price { get; set; }

        public decimal quantity { get; set; }
        public decimal unfilledQuantity { get; set; }

        public long createdAt { get; set; }
        public long updatedAt { get; set; }
    }
}
