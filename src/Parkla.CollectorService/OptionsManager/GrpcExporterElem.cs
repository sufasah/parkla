using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Generators;
using Parkla.CollectorService.Options;
using static Parkla.Protobuf.Collector;

namespace Parkla.CollectorService.OptionsManager;
public class GrpcExporterElem : ExporterElemBase
{
    private readonly Type _exporterBaseType = typeof(GrpcExporter); 
    public readonly static List<GrpcExporterElem> GrpcExporters = new();
    public string Group { get; set; }
    public CollectorClient? Client { get; set; }
    
    public override void ConfigureExporter(ExporterOptions exporter, ExporterBase exporterReference)
    {
        if (string.IsNullOrWhiteSpace(exporter.Address))
            throw new ArgumentNullException("Address", "GrpcExporter address value must be given");
        if (string.IsNullOrWhiteSpace(exporter.Group))
            throw new ArgumentNullException("Group", "GrpcExporter group value must be given");

        Group = exporter.Group;
        ExporterReference = exporterReference;
        
        var client = GrpcClientPool.GetInstance(exporter.Address);
        if(client != null) {
            Client = client;
            GrpcExporters.Add(this);
        }
    }

    public override Type GetExporterBaseType()
    {
        return _exporterBaseType;
    }
}