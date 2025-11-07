using System.Diagnostics;
using EShop.Contracts;
using EShop.OrderService.Clients;
using EShop.OrderService.Database;
using EShop.OrderService.Models;
using MassTransit;

namespace EShop.OrderService.Services;

public class OrderService(
    WarehouseClient client,
    AppDbContext context,
    IPublishEndpoint publishEndpoint)
{
    public async Task<Order> CreateOrder(int productId, int quantity)
    {
        using var activity = Telemetry.Source.StartActivity();

        Telemetry.TryOrdersCounter.Add(1);

        activity?.AddEvent(new ActivityEvent("Reserve product"));
        var product = await client.ReserveProduct(productId, quantity);
        activity?.AddEvent(new ActivityEvent("Product reserved"));

        var order = new Order
        {
            OrderId = Guid.NewGuid(),
            OrderDate = DateTime.UtcNow,
            OrderItems =
            [
                new OrderItem
                {
                    ProductId = product.ProductId,
                    Quantity = product.Quantity,
                    ProductName = product.ProductName,
                    ProductPrice = product.ProductPrice
                }
            ]
        };
        await context.Save(order);

        Telemetry.SuccessOrdersCounter.Add(1);
        await SendEvent(order);
        return order;
    }

    private async Task SendEvent(Order order)
    {
        await publishEndpoint.Publish<IOrderCreatedEvent>(new
        {
            order.OrderId,
            OrderItemsCount = order.OrderItems.Count
        });
    }
}