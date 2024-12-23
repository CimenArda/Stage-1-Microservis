using MassTransit;
using Shared.Event;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        readonly IPublishEndpoint _publishEndpoint;

        public StockReservedEventConsumer(IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public Task Consume(ConsumeContext<StockReservedEvent> context)
        {

            //Ödeme İşlemleri
            if (true)
            {
                PaymentCompletedEvent paymentCompletedEvent = new PaymentCompletedEvent()
                {
                    OrderID = context.Message.OrderID,

                };
                _publishEndpoint.Publish(paymentCompletedEvent);
                //Ödeme başarıyla tamamlandıgı zaman...
            }
            else
            {
                PaymentFailedEvent paymentFailedEvent = new PaymentFailedEvent()
                {
                    OrderID = context.Message.OrderID,
                    Message = "Bakiye Yetersiz"
                };

                //Ödemede sıkıntı oldugu zaman...

            }

            return Task.CompletedTask;

        }
    }
}
