using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_A.Context;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<Service_AContext>();


builder.Services.AddHttpClient("Service_B",httpclient=>
{
    httpclient.BaseAddress = new Uri("https://localhost:7297");
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
app.MapGet("/{id}/{newName}", async (
    [FromRoute] string id,
    [FromRoute] string newName,
    Service_AContext dbContext, 
    IHttpClientFactory httpClientFactory) =>
{
    var httpClient = httpClientFactory.CreateClient("Service_B");

    // Veritabanýndan kiþiyi bul
    var person = await dbContext.Persons.FirstOrDefaultAsync(p => p.PersonID == int.Parse(id));
    if (person == null)
    {
        return Results.NotFound("Person not found");
    }

    // Ýsmi güncelle
    person.Name = newName;
    dbContext.Persons.Update(person);

    // Deðiþiklikleri kaydet
    await dbContext.SaveChangesAsync();

    // Diðer servisi çaðýr
    var httpResponseMessage = await httpClient.GetAsync($"update/{person.PersonID}/{person.Name}");
    if (httpResponseMessage.IsSuccessStatusCode)
    {
        var content = await httpResponseMessage.Content.ReadAsStringAsync();
        await Console.Out.WriteLineAsync(content);
    }

    return Results.Ok("Person updated successfully");
});
app.Run();
