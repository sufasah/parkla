using System.IO.Ports;
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Options;
using Parkla.CollectorService.Receivers;

namespace Parkla.CollectorService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOptionsMonitor<CollectorOptions> _collectorOptionsMonitor;
    private CancellationTokenRegistration Registration;
    private readonly SerialReceiver _serialReceiver;

    public Worker(
        ILogger<Worker> logger, 
        IOptionsMonitor<CollectorOptions> collectorOptionsMonitor,
        SerialReceiver serialReceiver)
    {
        _logger = logger;
        _collectorOptionsMonitor = collectorOptionsMonitor;
        _serialReceiver = serialReceiver;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _serialReceiver.RegisterOpitons(_collectorOptionsMonitor.CurrentValue);
    }
}
