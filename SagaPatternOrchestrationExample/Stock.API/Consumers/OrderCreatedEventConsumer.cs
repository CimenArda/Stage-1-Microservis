﻿using MassTransit;
using MassTransit.Transports;
using Shared.OrderEvents;
using Shared.Settings;
using Shared.StockEvents;
using Stock.API.Services;
using MongoDB.Driver;
namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly MongoDbService _mongoDbService;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        public OrderCreatedEventConsumer(MongoDbService mongoDbService, ISendEndpointProvider sendEndpointProvider)
        {
            _mongoDbService = mongoDbService;
            _sendEndpointProvider = sendEndpointProvider;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            List<bool> stockResults = new();
            var stockCollection = _mongoDbService.GetCollection<Stock.API.Models.Stock>();

            foreach (var orderItem in context.Message.OrderItems)
                stockResults.Add(await(await stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId && s.Count >=
                (long)orderItem.Count)).AnyAsync());

            var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMQSettings.StateMachineQueue}"));
            if (stockResults.TrueForAll(s => s.Equals(true)))
            {
                foreach (var orderItem in context.Message.OrderItems)
                {
                    var stock = await(await stockCollection.FindAsync(s => s.ProductId == orderItem.ProductId)).FirstOrDefaultAsync();

                    stock.Count -= orderItem.Count;

                    await stockCollection.FindOneAndReplaceAsync(x => x.ProductId == orderItem.ProductId, stock);
                }

                StockReservedEvent stockReservedEvent = new(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems
                };

                await sendEndpoint.Send(stockReservedEvent);
            }
            else
            {
                StockNotReservedEvent stockNotReservedEvent = new(context.Message.CorrelationId)
                {
                    Message = "Stok yetersiz...."
                };

                await sendEndpoint.Send(stockNotReservedEvent);
            }
        }
    }
  }