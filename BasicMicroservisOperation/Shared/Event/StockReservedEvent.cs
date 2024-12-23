using Shared.Event.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Event
{
    public class StockReservedEvent :IEvent
    {
        public Guid BuyerID { get; set; }
        public Guid OrderID { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
