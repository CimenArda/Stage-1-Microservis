using Order.API.Models.Entities;
using Order.API.Models.Enums;

namespace Order.API.ViewModels
{
    public class CreateOrderViewModel
    {
        public Guid BuyerID { get; set; }
        public List<CreateOrderItemViewModel> OrderItems { get; set; }
    }
}
