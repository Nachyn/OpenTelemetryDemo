using System.Diagnostics;
using EShop;
using EShop.Clients;
using EShop.Database;
using EShop.Middlewares;
using EShop.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddScoped<AppDbContext>();
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<WarehouseClient>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.MapPost("/api/orders",
        ([FromServices] OrderService service,
            [FromQuery] int productId,
            [FromQuery] int quantity) =>
        {
            var activity = Diagnostic.Source.StartActivity("API handler: /api/orders");
            activity?.AddEvent(new ActivityEvent("OrderService call started"));
            var order = service.CreateOrder(productId, quantity);
            activity?.AddEvent(new ActivityEvent("OrderService call completed"));
            return order;
        })
    .WithName("GetWeatherForecast");

app.Run();