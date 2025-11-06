using System.Diagnostics;
using EShop.OrderService;
using EShop.OrderService.Clients;
using EShop.OrderService.Database;
using EShop.OrderService.Middlewares;
using EShop.OrderService.Services;
using MassTransit;
using MassTransit.Logging;
using Microsoft.AspNetCore.Mvc;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.OpenTelemetry(options =>
    {
        options.Protocol = OtlpProtocol.Grpc;
        options.ResourceAttributes = new Dictionary<string, object>
        {
            ["EnvNameTest"] = Diagnostic.GlobalSystemName,
            ["service.name"] = Diagnostic.ApplicationName
        };
    })
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddMassTransit(x =>
    {
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host("localhost", "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.ConfigureEndpoints(context);
        });
    });

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSerilog();

    builder.Services.AddHttpClient();
    builder.Services.AddScoped<AppDbContext>();
    builder.Services.AddScoped<OrderService>();
    builder.Services.AddScoped<WarehouseClient>();

    builder.Services.AddOpenTelemetry()
        .ConfigureResource(x => x
            .AddService(Diagnostic.ApplicationName)
            .AddAttributes([
                new KeyValuePair<string, object>("EnvNameTest", Diagnostic.GlobalSystemName)
            ]))
        .WithTracing(b => b
            .AddSource(Diagnostic.InstrumentsSourceName) // My ActivitySource
            .AddSource(DiagnosticHeaders.DefaultListenerName) // MassTransit ActivitySource
            .AddAspNetCoreInstrumentation(o =>
            {
                o.RecordException = true;
                o.Filter = httpContext =>
                {
                    var pathValue = httpContext.Request.Path.Value;
                    return pathValue is null || (pathValue != "/metrics" &&
                                                 !pathValue.StartsWith("/swagger") &&
                                                 !pathValue.StartsWith("/_vs") &&
                                                 !pathValue.StartsWith("/framework"));
                };
            })
            .AddHttpClientInstrumentation()
            .AddOtlpExporter())
        .WithMetrics(x => x
            .AddMeter(Diagnostic.InstrumentsSourceName)
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddRuntimeInstrumentation()
            .AddProcessInstrumentation()
            .AddOtlpExporter((options, readerOptions) =>
            {
                readerOptions.PeriodicExportingMetricReaderOptions.ExportIntervalMilliseconds =
                    1000;
            }));

    var app = builder.Build();

    app.UseMiddleware<ExceptionMiddleware>();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    // app.UseHttpsRedirection();

    app.MapPost("/api/orders",
            ([FromServices] OrderService service,
                [FromQuery] int productId,
                [FromQuery] int quantity) =>
            {
                using var activity = Diagnostic.Source.StartActivity("API handler: /api/orders");
                activity?.AddEvent(new ActivityEvent("OrderService call started"));
                var order = service.CreateOrder(productId, quantity);
                activity?.AddEvent(new ActivityEvent("OrderService call completed"));
                return order;
            })
        .WithName("EShop.Orders");

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}