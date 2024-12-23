using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceA.Context;
using ServiceB.Context;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ServiceAContext>();
builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, _configurator) =>
    {
        _configurator.Host(builder.Configuration["RabbitMQ"]);
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
app.MapGet("updateName/{id}/{newName}", async (
    [FromRoute] int id, // MSSQL'de ID genellikle int t�r�ndedir
    [FromRoute] string newName,
    ServiceAContext dbContext, // MSSQL ba�lam�
    IPublishEndpoint publishEndpoint) =>
{
    // Veritaban�ndan Person kayd�n� bul
    var person = await dbContext.Persons.FirstOrDefaultAsync(p => p.PersonID == id);
    if (person == null)
    {
        return Results.NotFound("Person not found");
    }

    // �smi g�ncelle
    person.Name = newName;

    // De�i�iklikleri kaydet
    dbContext.Persons.Update(person);
    await dbContext.SaveChangesAsync();

    // RabbitMQ'ya UpdatedPersonNameEvent g�nder
    UpdatedPersonNameEvent updatedPersonNameEvent = new()
    {
        PersonId = id,
        NewName = newName
    };

    await publishEndpoint.Publish(updatedPersonNameEvent);

    return Results.Ok("Person updated and event published successfully");
});
app.Run();
