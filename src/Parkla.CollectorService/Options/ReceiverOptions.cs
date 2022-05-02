using Parkla.CollectorService.Library;

namespace Parkla.CollectorService.Options
{
    public class ReceiverOptions
    {
        public ReceiverType Type { get; set; }
        public string Handler { get; set; }

        // -------------------------------- SERİAL
        public string PortName { get; set; }
        
        
        // -------------------------------- HTTP
        public string Endpoint { get; set; }


        // -------------------------------- GRPC
        public string Group { get; set; }

    }
}