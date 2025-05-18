using EShop.Clients;
using EShop.Database;
using EShop.Models;

namespace EShop.Services;

public class OrderService(WarehouseClient client, AppDbContext context)
{
    public async Task<Order> CreateOrder(int productId, int quantity)
    {
        using var activity = Diagnostic.Source.StartActivity();

        activity?.AddEvent(new ("Reserve product"));
        var product = await client.ReserveProduct(productId, quantity);
        activity?.AddEvent(new ("Product reserve"));

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

        return order;
    }
}