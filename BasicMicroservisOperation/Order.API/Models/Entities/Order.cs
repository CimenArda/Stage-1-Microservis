using Order.API.Models.Enums;

namespace Order.API.Models.Entities
{
    public class Order
    {
        public Guid OrderID { get; set; }
        public Guid BuyerID { get; set; }

        public decimal TotalPrice { get; set; }

        public OrderStatus OrderStatus { get; set; }     

        public DateTime CreatedDate { get; set; }

        public List<OrderItem> OrderItems { get; set; }





    }
}
