using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Options;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Receivers;

public abstract class ReceiverBase : IDisposable
{
    private readonly ILogger _logger;
    private readonly HttpExporter _httpExporter;
    private readonly SerialExporter _serialExporter;
    private bool disposed = false;

    public ReceiverBase(
        ILogger logger,
        HttpExporter httpExporter,
        SerialExporter serialExporter
    )
    {
        _logger = logger;
        _httpExporter = httpExporter;
        _serialExporter = serialExporter;
    }
    protected void ExportResults(
        IEnumerable<ParkSpaceStatusDto> results, 
        IEnumerable<HttpExporterOptions> httpExporters, 
        IEnumerable<SerialExporterOptions> serialExporters
    ) {
        foreach (var result in results) {
            var tasks = new List<Task>();

            _logger.LogInformation("ParkId='{}', SpaceId='{}', Status='{}' is being exported with all exporters in the pipeline", result.Parkid, result.Spaceid, result.Status);

            foreach (var exporter in httpExporters) {
                tasks.Add(_httpExporter.ExportAsync(result, exporter.Url));
            }

            foreach (var exporter in serialExporters) {
                _serialExporter.Enqueue(result, exporter.PortName);
            }

            Task.WhenAll(tasks).ContinueWith((task) => {
                _logger.LogInformation("ParkId='{}', SpaceId='{}', Status='{}' is exported with all exporters in the pipeline", result.Parkid, result.Spaceid, result.Status);
            });
        }
    }

    public void Dispose(bool disposing) {
        if(disposed) {
            return;
        }

        if(disposing) {
            _httpExporter.Dispose();
            _serialExporter.Dispose();
        }

        disposed = true;
    }

    public void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~ReceiverBase() {
        Dispose(false);
    }
}
