
using Parkla.Core.DTOs;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.Exporters;
public class ExportManager
{
    private readonly HttpExportManager _httpExportManager;
    private readonly SerialExportManager _serialExportManager;

    public ExportManager(
        HttpExportManager httpExportManager,
        SerialExportManager serialExportManager)
    {
        _httpExportManager = httpExportManager;
        _serialExportManager = serialExportManager;
    }

    public void Export(ExporterOptions exporter, IEnumerable<ParkSpaceStatusDto> dtos) {
        foreach(var dto in dtos)
            Export(exporter,dto);
    }

    public void Export(ExporterOptions exporter, ParkSpaceStatusDto dto) {
        if(exporter.Type == ExporterType.HTTP) {
            var httpExporter = (HttpExporterOptions) exporter;
            _httpExportManager.ExportAsync(dto, httpExporter.Url);
        }
        else if(exporter.Type == ExporterType.SERIAL) {
            var serialExporter = (SerialExporterOptions) exporter;
            _serialExportManager.Enqueue(dto, serialExporter.PortName);
        }
    }

    
}