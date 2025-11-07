using EShop.Contracts;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace EShop.Notification.Consumers;

public sealed class OrderCreatedConsumer(ILogger<OrderCreatedConsumer> logger) : IConsumer<IOrderCreatedEvent>
{
    public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
    {
        var msg = context.Message;
        logger.LogInformation($"START consume order with id {msg.OrderId}");

        using (var activity = Telemetry.Source.StartActivity("Consume OrderCreated: Handle part 1"))
        {
            logger.LogInformation("Handling part 1..");
            activity?.AddTag("OrderItemsCount", msg.OrderItemsCount);
            await RandomDelay();

            using (Telemetry.Source.StartActivity("Consume OrderCreated: Handle part 2"))
            {
                logger.LogInformation("Handling part 2..");
                await RandomDelay();
            }
        }

        logger.LogInformation($"END consume order with id {msg.OrderId}");
    }

    private async Task RandomDelay()
    {
        // [CallerMemberName] = RandomDelay
        using var _ = Telemetry.Source.StartActivity();

        var random = new Random();
        await Task.Delay(random.Next(100, 1000));
    }
}