using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.OptionsManager;
public class HttpExporterElem : ExporterElemBase
{
    private readonly Type _exporterBaseType = typeof(HttpExporter); 
    public readonly static List<HttpExporterElem> HttpExporters = new();
    public Uri? Url { get; set; }

    public override void ConfigureExporter(ExporterOptions exporter, ExporterBase exporterReference)
    {
        if(string.IsNullOrWhiteSpace(exporter.Url.OriginalString))
            throw new ArgumentNullException("Url", "HttpExporter Url value must be a valid http url");

        Url = exporter.Url;
        ExporterReference = exporterReference;
        HttpExporters.Add(this);
    }
    public override Type GetExporterBaseType()
    {
        return _exporterBaseType;
    }
}