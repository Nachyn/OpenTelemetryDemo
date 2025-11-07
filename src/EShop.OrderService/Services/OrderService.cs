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

        try
        {
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

            Telemetry.OrdersSuccessTotal.Add(1);
            Telemetry.ProductsPerOrderHistogram.Record(order.OrderItems.Sum(i => i.Quantity));
            activity?.AddEvent(new ActivityEvent("Order saved to database"));

            await SendEvent(order);
            return order;
        }
        catch (Exception ex)
        {
            // Отмечаем неуспешную попытку
            Telemetry.OrdersFailedTotal.Add(1);

            // Добавляем ошибку в трейсы
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
            activity?.AddEvent(new ActivityEvent($"Error: {ex.Message}"));

            throw;
        }
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