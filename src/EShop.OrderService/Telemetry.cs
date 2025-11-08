using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;

namespace EShop.OrderService;

public static class Telemetry
{
    public const string GlobalSystemName = "EShop";
    public const string ApplicationName = "EShop.OrderService";
    public const string InstrumentsSourceName = "EShop.OrderService";
    public static readonly ActivitySource Source = new(InstrumentsSourceName);

    private static readonly Meter Meter = new(InstrumentsSourceName);

    public static readonly Counter<int> OrdersSuccessTotal =
        Meter.CreateCounter<int>(
            name: "orders_success_total",
            unit: "count",
            description: "Number of successfully created orders"
        );

    public static readonly Counter<int> OrdersFailedTotal =
        Meter.CreateCounter<int>(
            name: "orders_failed_total",
            unit: "count",
            description: "Number of failed order attempts"
        );

    public static readonly Histogram<int> ProductsPerOrderHistogram =
        Meter.CreateHistogram("orders_items_per_order", unit: "count",
            description: "Distribution of number of items per order",
            advice: new InstrumentAdvice<int>
                {HistogramBucketBoundaries = [1, 2, 3, 5, 8, 10]});
}