using System.Diagnostics;

namespace EShop.Notification;

public static class Diagnostic
{
    public const string GlobalSystemName = "EShop";
    public const string ApplicationName = "EShop.NotificationService";
    public const string InstrumentsSourceName = "EShop.NotificationService";

    public static readonly ActivitySource Source = new(InstrumentsSourceName);
}