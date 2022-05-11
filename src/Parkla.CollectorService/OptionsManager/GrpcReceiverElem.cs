using System.Reflection;
using Parkla.CollectorService.Handlers;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.OptionsManager;
public class GrpcReceiverElem : ReceiverElemBase
{
    public readonly static List<Pipe> GrpcPipes = new();
    public string? Group { get; set; }

    public override void ConfigureReceiver(ReceiverOptions receiver, ExporterElemBase[] exporters, Assembly pluginAssembly)
    {
        if (string.IsNullOrWhiteSpace(receiver.Group))
            throw new ArgumentNullException("Group", "GrpcReceiver Group configuration value must be given");

        Group = receiver.Group;
        Handler = GetHandler(receiver.Handler, typeof(DefualtGrpcHandler), pluginAssembly);

        GrpcPipes.Add(new(){
            Receiver = this,
            Exporters = exporters
        });
    }
}