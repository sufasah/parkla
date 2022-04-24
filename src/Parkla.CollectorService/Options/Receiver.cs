using Parkla.CollectorService.Enums;

namespace Parkla.CollectorService.Options
{
    public abstract class Receiver
    {
        public ReceiverType Type { get; set; }
        public string Handler { get; set; }
    }
}