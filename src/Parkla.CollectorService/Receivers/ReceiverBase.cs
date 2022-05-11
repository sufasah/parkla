using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.OptionsManager;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Receivers;

public abstract class ReceiverBase
{
    private readonly ILogger _logger;
    private readonly object _startLock = new();
    public bool Started { get; private set; } = false;

    public ReceiverBase(
        ILogger logger
    )
    {
        _logger = logger;
    }

    public void Start() {
        lock (_startLock) {
            var typeName = GetType().Name;
            if (Started) {
                _logger.LogWarning("{}.Start: {} is already started.",typeName, typeName);
                return;
            }

            DoStart();
        
            Started = true;
            _logger.LogInformation("START: {} is started", typeName);
        }
    }

    protected abstract void DoStart();

    protected async Task ExportResultsAsync(
        IEnumerable<ParkSpaceStatusDto> results,
        ExporterElemBase[] exporters
    ) {
        var tasks = new List<Task>();
        
        foreach (var exporter in exporters) {
            var task = exporter.ExporterReference!.ExportAsync(results, exporter);
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }
}
