namespace EShop.Dto;

public record OrderItemDto(
    int ProductId,
    string ProductName,
    int ProductPrice,
    int Quantity
);