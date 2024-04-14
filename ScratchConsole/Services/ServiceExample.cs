using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ScratchConsole.Services;

internal class ServiceExample(
    ILogger<ServiceExample> logger,
    IHostApplicationLifetime appLifetime)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Service work goes here...
        logger.LogInformation("Service started");
        await Task.Delay(5000, cancellationToken);

        appLifetime.StopApplication();
    }
}