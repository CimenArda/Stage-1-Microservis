using Shared.Event.Common;
using Shared.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Event
{
    public class OrderCreatedEvent :IEvent
    {
        public Guid OrderID { get; set; }

        public Guid BuyerID { get; set; }

        public List<OrderItemMessage> OrderItems { get; set; }

        public decimal TotalPrice { get; set; }

    }
}
