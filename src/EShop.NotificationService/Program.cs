using EShop.Notification;
using EShop.Notification.Consumers;
using MassTransit;
using MassTransit.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        x.AddConsumer<OrderCreatedConsumer>();

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

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

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