using Microsoft.Extensions.Hosting;

namespace BackgroundWorkers.Reconciliation;

public sealed class PaytrReconciliationWorker(ILogger<PaytrReconciliationWorker> log) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        log.LogInformation("Reconciliation worker started");
        // PayTR raporlarını indir → payments/ledger ile mutabakat
        await Task.CompletedTask;
    }
}
