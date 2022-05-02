namespace Parkla.CollectorService.Options
{
    public class PipelineOptions
    {
        public ReceiverOptions[] Receivers { get; set; }
        public ExporterOptions[] Exporters { get; set; }
    }
}