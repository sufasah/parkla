using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.OptionsManager;
public class Pipe
{
    public ReceiverElemBase Receiver { get; set; }
    public ExporterElemBase[] Exporters { get; set; }
}