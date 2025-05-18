using EShop.Dto;

namespace EShop.Clients;

public class WarehouseClient(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _client = httpClientFactory.CreateClient();

    public async Task<OrderItemDto> ReserveProduct(int productId, int quantity)
    {
        using var activity = Diagnostic.Source.StartActivity();
        activity?.AddTag("productId", productId);
        activity?.AddTag("quantity", productId);

        await _client.GetAsync("https://jsonplaceholder.typicode.com/todos/1");
        return new OrderItemDto(productId, "IPhone X", 59000, quantity);
    }
}