
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

    public void Export(Exporter exporter, IEnumerable<ParkSpaceStatusDto> dtos) {
        if(exporter.Type == ExporterType.HTTP) {
            var httpExporter = (HttpExporter) exporter;
            foreach(var dto in dtos){
                _httpExportManager.ExportAsync(dto, httpExporter.Url);
            }
        }
        else if(exporter.Type == ExporterType.SERIAL) {
            var serialExporter = (SerialExporter) exporter;
            foreach(var dto in dtos) {
                _serialExportManager.Export(dto, serialExporter.PortName);
            }
        }
    }

    public void Export(Exporter exporter, ParkSpaceStatusDto dto) {
        Export(exporter, new ParkSpaceStatusDto[]{dto});
    }

    
}