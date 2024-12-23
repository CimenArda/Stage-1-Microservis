using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.API.Consumers;
using Order.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();




builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<OrderAPIDbContext>(opt=>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer"));
});

builder.Services.AddMassTransit(confg =>
{
    confg.AddConsumer<PaymentCompletedEventConsumer>();
    confg.AddConsumer<StockNotReservedEventConsumer>();
    confg.AddConsumer<PaymentFailedEventConsumer>();

    confg.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host("amqps://zmohqwps:2E2lCfk_9olDcVer7FAFesF0w_z_3lgo@shrimp.rmq.cloudamqp.com/zmohqwps"); //cloudamqp

        _configurator.ReceiveEndpoint("order-payment-completed-event-queue",e=>e.ConfigureConsumer<PaymentCompletedEventConsumer>(context));
        _configurator.ReceiveEndpoint("order-stock-failed-event-queue", e => e.ConfigureConsumer<StockNotReservedEventConsumer>(context));
        _configurator.ReceiveEndpoint("order-payment-failed-event-queue", e => e.ConfigureConsumer<PaymentFailedEventConsumer>(context));
    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
