﻿using MassTransit;
using MassTransit.Transports;
using MongoDB.Driver;
using Shared;
using Shared.Events;
using Stock.API.Services;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly MongoDbService _mongoDbService;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(MongoDbService mongoDbService, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _mongoDbService = mongoDbService;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResult = new();
            IMongoCollection<Models.Stock> collection = _mongoDbService.GetCollection<Models.Stock>();

            foreach (var orderItem in context.Message.OrderItems)
            {
                stockResult.Add(await (await collection.FindAsync(s => s.ProductId == orderItem.ProductId.ToString() && s.Count > (long)orderItem.Count)).AnyAsync());
            }
            if (stockResult.TrueForAll(s => s.Equals(true)))
            {
                //Stock güncellemesi...
                foreach (var orderItem in context.Message.OrderItems)
                {
                    Models.Stock stock = await (await collection.FindAsync(s => s.ProductId == orderItem.ProductId.ToString())).FirstOrDefaultAsync();
                    stock.Count -= orderItem.Count;

                    await collection.FindOneAndReplaceAsync(x => x.ProductId == orderItem.ProductId.ToString(), stock);
                }

                //Payment'ı uyaracak event'in fırlatılması...
                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.Payment_StockReservedEvent}"));
                StockReservedEvent stockReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    TotalPrice = context.Message.TotalPrice,
                    OrderItems = context.Message.OrderItems,
                };
                await sendEndpoint.Send(stockReservedEvent);
            }
            else
            {
                //Stok miktarı ürünleri karsılayamıyor.(Stok İşlemi başarısız)
                //Order'ı uyaracak evet fırlatılacaktır.
                StockNotReservedEvent stockNotReservedEvent = new()
                {
                    BuyerId = context.Message.BuyerId,
                    OrderId = context.Message.OrderId,
                    Message = "Stok miktarı yetersiz..."
                };

                await _publishEndpoint.Publish(stockNotReservedEvent);
            }



            }





    }
}
