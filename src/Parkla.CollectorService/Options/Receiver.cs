using Parkla.CollectorService.Library;

namespace Parkla.CollectorService.Options
{
    public abstract class Receiver
    {
        public ReceiverType Type { get; set; }
        public Type Handler { get; set; }
    }
}