namespace EShop.Contracts;

public interface IOrderCreatedEvent
{
    public Guid OrderId { get; }
    public int OrderItemsCount { get; }
}