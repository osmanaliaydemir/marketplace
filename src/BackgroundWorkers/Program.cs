using BackgroundWorkers.Reconciliation;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<PaytrReconciliationWorker>();
var app = builder.Build();
await app.RunAsync();
