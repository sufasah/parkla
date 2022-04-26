namespace Parkla.CollectorService.Options
{
    public class PipelineOptions
    {
        public HttpReceiverOptions[] HttpReceivers { get; set; }
        public SerialReceiverOptions[] SerialReceivers { get; set; }
        public HttpExporterOptions[] HttpExporters { get; set; }
        public SerialExporterOptions[] SerialExporters { get; set; }
    }
}