
using Parkla.Core.DTOs;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.Exporters;
public class ExportManager
{
    private readonly HttpExporter _httpExporter;
    private readonly SerialExporter _serialExporter;

    public ExportManager(
        HttpExporter httpExporter,
        SerialExporter serialExporter)
    {
        _httpExporter = httpExporter;
        _serialExporter = serialExporter;
    }

    public void Export(ExporterOptions exporter, IEnumerable<ParkSpaceStatusDto> dtos) {
        foreach(var dto in dtos)
            Export(exporter,dto);
    }

    public void Export(ExporterOptions exporter, ParkSpaceStatusDto dto) {
        if(exporter.Type == ExporterType.HTTP) {
            var httpExporter = (HttpExporterOptions) exporter;
            _httpExporter.ExportAsync(dto, httpExporter.Url);
        }
        else if(exporter.Type == ExporterType.SERIAL) {
            var serialExporter = (SerialExporterOptions) exporter;
            _serialExporter.Enqueue(dto, serialExporter.PortName);
        }
    }

    
}