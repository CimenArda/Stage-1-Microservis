using MassTransit;
using MongoDB.Driver;
using Shared.Event;
using Shared.Messages;
using Stock.API.Models.Entities;
using Stock.API.Services.MongoDBServices;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        IMongoCollection<Models.Entities.Stock> _stockCollection;
        readonly ISendEndpointProvider _sendEndpointProvider;
        readonly IPublishEndpoint _publishEndpoint;
        public OrderCreatedEventConsumer(MongoDbService mongoDbService, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {

            _stockCollection = mongoDbService.GetCollection<Stock.API.Models.Entities.Stock>();
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new List<bool>();
            foreach (OrderItemMessage orderItem in context.Message.OrderItems)
            {
              stockResult.Add((await _stockCollection.FindAsync(s => s.ProductID == orderItem.ProductId && s.Count >= orderItem.Count)).Any());
            }

            if (stockResult.TrueForAll(sr=>sr.Equals(true)))
            {
                foreach (OrderItemMessage item in context.Message.OrderItems)
                {

                    Stock.API.Models.Entities.Stock stock =await(await _stockCollection.FindAsync(s => s.ProductID == item.ProductId)).FirstOrDefaultAsync();

                    stock.Count -= item.Count;
                    await _stockCollection.FindOneAndReplaceAsync(s => s.ProductID == item.ProductId, stock);


                }
                StockReservedEvent stockReservedEvent = new()
                {
                    TotalPrice = context.Message.TotalPrice,
                    BuyerID = context.Message.BuyerID,
                    OrderID = context.Message.OrderID
                };


             ISendEndpoint sendEndpoint =await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:payment-stock-reserved-event-queue"));
                await sendEndpoint.Send(stockReservedEvent);

                //Payment service'e stok işlemlerin tamamlandıgına dair bir event olusturup yolluyacağız.YUKARDA ŞUAN YAPTIK

            }
            else
            {

                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    BuyerID = context.Message.BuyerID,
                    OrderID = context.Message.OrderID,
                    Message = ""
                    //İstenilen Mesaj dönülebilir.
                };
                await _publishEndpoint.Publish(stockNotReservedEvent);
            


                //Sipariş tutarsız/geçersiz olduguna dair işlemler 
            }

        }
    }
}
