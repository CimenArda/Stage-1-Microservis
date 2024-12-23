using Coordinator.Models.Context;
using Coordinator.Services.Abstractions;
using Coordinator.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TwoPhaseCommitContext>
    (opt=>opt.UseSqlServer(builder.Configuration.GetConnectionString("SQLServer")));


builder.Services.AddHttpClient("OrderApi",client =>client.BaseAddress = new("https://localhost:7265/"));
builder.Services.AddHttpClient("StockApi",client =>client.BaseAddress = new("https://localhost:7188"));
builder.Services.AddHttpClient("PaymentApi",client =>client.BaseAddress = new("https://localhost:7221"));



builder.Services.AddTransient<ITransactionService, TransactionService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/create-order-transaction", async (ITransactionService transactionService) =>
{
    //Phase 1 - Prepare
    var transactionId = await transactionService.CreateTransactionAsync();
    await transactionService.PrepareServicesAsync(transactionId);
    bool transactionState = await transactionService.CheckReadyServicesAsync(transactionId);

    if (transactionState)
    {
        //Phase 2 - Commit
        await transactionService.CommitAsync(transactionId);
        transactionState = await transactionService.CheckTransactionStateServicesAsync(transactionId);
    }

    if (!transactionState)
        await transactionService.RoolbackAsync(transactionId);
});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
