using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace EShop;

public static class Diagnostic
{
    public const string GlobalSystemName = "EShop";
    public const string ApplicationName = "EShop.OrderService";
    public const string InstrumentsSourceName = "EShop.OrderService";
    public static readonly ActivitySource Source = new(InstrumentsSourceName);

    private static readonly Meter Meter = new(InstrumentsSourceName);

    public static readonly Counter<int> SuccessOrdersCounter =
        Meter.CreateCounter<int>("success-orders", "count", "the number of success orders");

    public static readonly Counter<int> TryOrdersCounter =
        Meter.CreateCounter<int>("try-orders", "count", "number of attempts to order");
}