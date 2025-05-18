namespace EShop.Database;

public class AppDbContext(ILogger<AppDbContext> logger)
{
    public async Task Save<T>(T entity)
    {
        // TODO: Use AddOpenTelemetry().WithTracing(x => x.AddEntityFrameworkCoreInstrumentation()...)

        using var activity = Diagnostic.Source.StartActivity(nameof(AppDbContext));
        var entityName = entity?.GetType().Name;
        activity?.AddTag("entity", entityName);
        logger.LogInformation("Start saving {entity}", entityName);

        var random = new Random();
        await Task.Delay(random.Next(100, 1000));

        logger.LogInformation("Entity saved {entity}", entityName);
    }
}