using MassTransit;
using MongoDB.Driver;
using Stock.API.Consumers;
using Stock.API.Services.MongoDBServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



builder.Services.AddMassTransit(confg =>
{

    confg.AddConsumer<OrderCreatedEventConsumer>();

    confg.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host("amqps://zmohqwps:2E2lCfk_9olDcVer7FAFesF0w_z_3lgo@shrimp.rmq.cloudamqp.com/zmohqwps"); //cloudamqp
        _configurator.ReceiveEndpoint("Stock_OrderCreatedEventQueue",e => e.ConfigureConsumer<OrderCreatedEventConsumer>(context));
    });
});

builder.Services.AddSingleton<MongoDbService>();

using IServiceScope scope =builder.Services.BuildServiceProvider().CreateScope();
MongoDbService mongodbService =scope.ServiceProvider.GetService<MongoDbService>();

var collection = mongodbService.GetCollection<Stock.API.Models.Entities.Stock>();


#region MongoDb'ye Seed Data Ekleme 
if (!collection.FindSync(s => true).Any())
{
    await collection.InsertOneAsync(new() { ProductID = Guid.NewGuid(),Count = 2000});
    await collection.InsertOneAsync(new() { ProductID = Guid.NewGuid(),Count = 3000});
    await collection.InsertOneAsync(new() { ProductID = Guid.NewGuid(),Count = 4000});
    await collection.InsertOneAsync(new() { ProductID = Guid.NewGuid(),Count = 6000});
    await collection.InsertOneAsync(new() { ProductID = Guid.NewGuid(),Count = 500});
}
#endregion



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
