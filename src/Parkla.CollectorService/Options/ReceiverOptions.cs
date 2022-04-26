using Parkla.CollectorService.Library;

namespace Parkla.CollectorService.Options
{
    public abstract class ReceiverOptions
    {
        public ReceiverType Type { get; set; }
        public HandlerBase Handler { get; set; }
    }
}