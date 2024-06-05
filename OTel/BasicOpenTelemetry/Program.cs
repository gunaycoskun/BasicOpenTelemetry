using BasicOpenTelemetry;
using BasicOpenTelemetry.Data;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<StudentDbContext>();
builder.Services.AddDbContext<StudentDbContext>(options =>
    options.UseInMemoryDatabase("StudentDb"));

Uri openTelemetryUri = new Uri("http://localhost:4317");
var openTelemetryConfig = !string.IsNullOrEmpty(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
if (openTelemetryConfig)
{
    openTelemetryUri = new Uri(builder.Configuration["OTEL_EXPORTER_OTLP_ENDPOINT"]);
}

builder.Services.AddOpenTelemetry()
    .ConfigureResource(res => res
        .AddService(DiagnosticsConfig.ServiceName))
    .WithMetrics(metrics =>
    {
        metrics
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation()
            .AddRuntimeInstrumentation();

        metrics.AddMeter(DiagnosticsConfig.Meter.Name);

        metrics.AddOtlpExporter(opt=> opt.Endpoint = openTelemetryUri);
    })
    .WithTracing(tracing =>
        {

            tracing
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddEntityFrameworkCoreInstrumentation();
            
            tracing.AddOtlpExporter(opt=> opt.Endpoint = openTelemetryUri);
            
        }
    );

builder.Logging.AddOpenTelemetry(log =>
{
    log.AddOtlpExporter(opt=> opt.Endpoint = openTelemetryUri);
    log.IncludeScopes = true;
    log.IncludeFormattedMessage = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
