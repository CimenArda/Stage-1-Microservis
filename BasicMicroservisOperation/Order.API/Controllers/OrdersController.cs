using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Order.API.Models;
using Order.API.Models.Entities;
using Order.API.ViewModels;
using Shared.Event;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderAPIDbContext _context;
        private readonly IPublishEndpoint _publishendpoint;

        public OrdersController(OrderAPIDbContext context, IPublishEndpoint endpoint)
        {
            _context = context;
            _publishendpoint = endpoint;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrder(CreateOrderViewModel createOrder)
        {
            Order.API.Models.Entities.Order order = new Models.Entities.Order()
            {
                OrderID = Guid.NewGuid(),
                BuyerID = createOrder.BuyerID,
                CreatedDate = DateTime.Now,
                OrderStatus = Models.Enums.OrderStatus.Suspend
            };
            order.OrderItems = createOrder.OrderItems.Select(oi => new OrderItem()
            {
                Count = oi.Count,
                Price = oi.Price,
                ProductId = oi.ProductId,


            }).ToList();

            order.TotalPrice = createOrder.OrderItems.Sum(oi => (oi.Price * oi.Count));

           await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();

            OrderCreatedEvent orderCreatedEvent = new()
            {
                BuyerID = order.BuyerID,
                OrderID = order.OrderID,
                OrderItems = order.OrderItems.Select(oi => new Shared.Messages.OrderItemMessage()
                {
                    Count = oi.Count,
                    ProductId = oi.ProductId,
                }).ToList(),
                TotalPrice = order.TotalPrice,
            };

          await _publishendpoint.Publish(orderCreatedEvent);





            return Ok("Başarılı bir şekilde Oluşturuldu");


        }
    }
}
