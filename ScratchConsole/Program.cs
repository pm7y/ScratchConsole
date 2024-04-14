using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ScratchConsole.Infrastructure;
using ScratchConsole.Services;

namespace ScratchConsole;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);
        var configuration = builder.Configuration;

        var settings = configuration.GetRequiredSection(Settings.SectionName)
                           .Get<Settings>(o => o.ErrorOnUnknownConfiguration = true) ??
                       throw new InvalidOperationException("Settings are required");
        builder.Services.AddSingleton(settings);
        builder.Services.ConfigureSerilog(configuration);
        builder.Services.ConfigureHostOptions();
        builder.Services.ConfigureDbContext("Default", configuration);

        builder.Services.AddSingleton<ConsoleRunner>();
        // builder.Services.AddHostedService<ServiceExample>();

        var host = builder.Build();

        var runner = host.Services.GetRequiredService<ConsoleRunner>();
        await runner.Execute(new CancellationTokenSource().Token);

        // await host.RunAsync();
    }
}