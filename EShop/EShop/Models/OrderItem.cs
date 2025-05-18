namespace EShop.Models;

public class OrderItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public int ProductPrice { get; set; }
    public int Quantity { get; set; }
}