namespace EShop.Models;

public class Order
{
    public Guid OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public List<OrderItem> OrderItems { get; set; } = [];
}