using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Outbox;

public sealed class OutboxProcessorHostedService(ILogger<OutboxProcessorHostedService> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        log.LogInformation("Outbox projector started");
        while (!stoppingToken.IsCancellationRequested)
        {
            // 1) Outbox'dan bekleyen mesajları oku
            // 2) Read modelleri/görevleri tetikle
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
