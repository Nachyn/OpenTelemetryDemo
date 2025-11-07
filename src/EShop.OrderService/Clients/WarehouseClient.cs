using EShop.OrderService.Dto;

namespace EShop.OrderService.Clients;

public class WarehouseClient(IHttpClientFactory httpClientFactory)
{
    private const string BaseUrl = "http://localhost:5000/api/warehouse";
    private readonly HttpClient _client = httpClientFactory.CreateClient();

    public async Task<OrderItemDto> ReserveProduct(int productId, int quantity)
    {
        using var activity = Telemetry.Source.StartActivity();
        activity?.AddTag("productId", productId);
        activity?.AddTag("quantity", productId);

        var requestUrl = $"{BaseUrl}?productId={productId}&quantity={quantity}";
        var response = await _client.PostAsync(requestUrl, null);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OrderItemDto>()
               ?? throw new NullReferenceException(nameof(ReserveProduct));
    }
}