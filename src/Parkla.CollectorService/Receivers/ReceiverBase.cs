using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Options;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Receivers;

public abstract class ReceiverBase
{
    private readonly ILogger _logger;
    private readonly HttpExporter _httpExporter;
    private readonly SerialExporter _serialExporter;
    private readonly GrpcExporter _grpcExporter;

    public ReceiverBase(
        ILogger logger,
        HttpExporter httpExporter,
        SerialExporter serialExporter,
        GrpcExporter grpcExporter
    )
    {
        _logger = logger;
        _httpExporter = httpExporter;
        _serialExporter = serialExporter;
        _grpcExporter = grpcExporter;
    }
    protected void ExportResults(
        IEnumerable<ParkSpaceStatusDto> results, 
        IEnumerable<HttpExporterOptions> httpExporters, 
        IEnumerable<SerialExporterOptions> serialExporters,
        IEnumerable<GrpcExporterOptions> grpcExporters
    ) {
        var tasks = new List<Task>();
        
        foreach (var exporter in httpExporters) {
            tasks.Add(_httpExporter.ExportAsync(results, exporter.Url));
        }
        
        foreach (var exporter in serialExporters) {
            _serialExporter.Enqueue(results, exporter.PortName);
        }

        foreach (var exporter in grpcExporters)
        {
            _grpcExporter.ExportAsync(results, exporter.Address);
        }
    }
}
