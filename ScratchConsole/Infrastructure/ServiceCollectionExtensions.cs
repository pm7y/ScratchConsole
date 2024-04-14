using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScratchConsole.Db;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.SystemConsole.Themes;

namespace ScratchConsole.Infrastructure;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureDbContext(
        this IServiceCollection services,
        string connectionStringName,
        IConfiguration configuration)
    {
        return services.AddDbContext<ConsoleDbContext>(o =>
        {
            o.EnableDetailedErrors();
            o.EnableSensitiveDataLogging();
            o.UseSqlServer(
                configuration.GetConnectionString(connectionStringName),
                options =>
                {
                    // Ensure EF core knows we are using the latest version of SQL Server
                    options.UseCompatibilityLevel(150);
                    // Use the new query splitting behavior by default
                    options.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
        }, ServiceLifetime.Singleton);
    }

    public static IServiceCollection ConfigureHostOptions(this IServiceCollection services)
    {
        return services.Configure<HostOptions>(hostOptions =>
            hostOptions.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost);
    }

    public static IServiceCollection ConfigureSerilog(this IServiceCollection services, IConfiguration config)
    {
        return services.AddSerilog((_, loggerConfiguration) =>
        {
            // Check if a console sink is configured.
            var consoleSinkConfigured = config
                .GetSection("Serilog:WriteTo")
                .GetChildren()
                .Any(s => s.GetValue<string>("Name") == "Console");

            loggerConfiguration
                .Enrich.FromLogContext()
                .Enrich.WithProperty("MachineName", Environment.MachineName)
                // Defaults, for when there are none in appSettings.
                .MinimumLevel.Is(LogEventLevel.Debug)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                .MinimumLevel.Override("System", LogEventLevel.Information)
                // Override defaults from configuration settings, if any.
                .ReadFrom.Configuration(config)
                // If no console sink is configured, add one.
                .WriteTo.Conditional(_ => !consoleSinkConfigured,
                    sinkConfiguration => sinkConfiguration.Console(theme: AnsiConsoleTheme.Code));
        }, true);
    }
}