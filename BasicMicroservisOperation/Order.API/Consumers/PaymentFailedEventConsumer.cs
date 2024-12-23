using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Models;
using Shared.Event;

namespace Order.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        readonly OrderAPIDbContext _orderAPIDbContext;

        public PaymentFailedEventConsumer(OrderAPIDbContext context)
        {
            _orderAPIDbContext = context;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var order = await _orderAPIDbContext.Orders.FirstOrDefaultAsync(o => o.OrderID == context.Message.OrderID);
            order.OrderStatus = Models.Enums.OrderStatus.Failed;
            await _orderAPIDbContext.SaveChangesAsync();
        }
    }
}
