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
    Service_BContext dbContext) => // MSSQL ba�lam�
{
    // PersonId'ye g�re Employee tablosundan kayd� bul
    var employee = await dbContext.Employees.FirstOrDefaultAsync(e => e.PersonID == personId);
    if (employee == null)
    {
        return Results.NotFound("Employee not found");
    }

    // �smi g�ncelle
    employee.Name = newName;

    // De�i�iklikleri kaydet
    dbContext.Employees.Update(employee);
    await dbContext.SaveChangesAsync();

    return Results.Ok(true); // Ba�ar�yla tamamland�
});
app.Run();
