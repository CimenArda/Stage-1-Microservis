namespace Order.API.ViewModels
{
    public class CreateOrderViewModel
    {
        public string BuyerId { get; set; }

        public List<CreateOrderItemViewModel> CreateOrderItemViewModels { get; set; }
    }
}
