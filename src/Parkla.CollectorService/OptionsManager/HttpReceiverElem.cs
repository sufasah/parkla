using System.Reflection;
using Parkla.CollectorService.Handlers;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.OptionsManager;
public class HttpReceiverElem : ReceiverElemBase
{
    public readonly static List<Pipe> HttpPipes = new();
    public string? Endpoint { get; set; }

    public override void ConfigureReceiver(ReceiverOptions receiver, ExporterElemBase[] exporters, Assembly assemblyPlugin)
    {
        if (string.IsNullOrWhiteSpace(receiver.Endpoint))
            throw new ArgumentNullException("Endpoint", "HttpReceiver Endpoint configuration value must be a valid http url");

        Endpoint = receiver.Endpoint;
        Handler = GetHandler(receiver.Handler, typeof(DefaultHttpHandler), assemblyPlugin);

        HttpPipes.Add(new() {
            Receiver = this,
            Exporters = exporters
        });
    }
}