using Parkla.CollectorService.Library;

namespace Parkla.CollectorService.Options
{
    public class ExporterOptions
    {
        public ExporterType Type { get; set; }

        // -------------------------------- SERİAL
        public string PortName { get; set; }


        // -------------------------------- HTTP
        public Uri Url { get; set; }


        // -------------------------------- GRPC
        public string Group { get; set; }
        public string Address { get; set; }
    }
}