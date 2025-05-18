using System.Diagnostics;
using EShop.Clients;
using EShop.Database;
using EShop.Models;

namespace EShop.Services;

public class OrderService(WarehouseClient client, AppDbContext context)
{
    public async Task<Order> CreateOrder(int productId, int quantity)
    {
        using var activity = Diagnostic.Source.StartActivity();

        Diagnostic.TryOrdersCounter.Add(1);

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

        Diagnostic.SuccessOrdersCounter.Add(1);
        return order;
    }
}