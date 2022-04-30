
using Parkla.Core.DTOs;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.Exporters;
public class ExportManager : IDisposable
{
    private readonly HttpExporter _httpExporter;
    private readonly SerialExporter _serialExporter;
    private bool disposed = false;

    public ExportManager(
        HttpExporter httpExporter,
        SerialExporter serialExporter)
    {
        _httpExporter = httpExporter;
        _serialExporter = serialExporter;
    }

    public async Task ExportAsync(ExporterOptions exporter, IEnumerable<ParkSpaceStatusDto> dtos) {
        var tasks = new List<Task>();

        foreach(var dto in dtos) {
            var task = ExportAsync(exporter,dto);
            tasks.Add(task);
        }
        
        await Task.WhenAll(tasks);
    }

    public async Task ExportAsync(ExporterOptions exporter, ParkSpaceStatusDto dto) {
        var tasks = new List<Task>();

        if(exporter.Type == ExporterType.HTTP) {
            var httpExporter = (HttpExporterOptions) exporter;
            var task = _httpExporter.ExportAsync(dto, httpExporter.Url);
            tasks.Add(task);
        }
        else if(exporter.Type == ExporterType.SERIAL) {
            var serialExporter = (SerialExporterOptions) exporter;
            _serialExporter.Enqueue(dto, serialExporter.PortName);
        }

        await Task.WhenAll(tasks);
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

    ~ExportManager() {
        Dispose(false);
    }
}