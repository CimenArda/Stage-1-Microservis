using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Service_B.Context;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<Service_BContext>();
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
app.MapGet("update/{personId}/{newName}", async (
    [FromRoute] int personId,
    [FromRoute] string newName,
    Service_BContext dbContext) => // MSSQL baðlamý
{
    // PersonId'ye göre Employee tablosundan kaydý bul
    var employee = await dbContext.Employees.FirstOrDefaultAsync(e => e.PersonID == personId);
    if (employee == null)
    {
        return Results.NotFound("Employee not found");
    }

    // Ýsmi güncelle
    employee.Name = newName;

    // Deðiþiklikleri kaydet
    dbContext.Employees.Update(employee);
    await dbContext.SaveChangesAsync();

    return Results.Ok(true); // Baþarýyla tamamlandý
});
app.Run();
