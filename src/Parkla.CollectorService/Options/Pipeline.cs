namespace Parkla.CollectorService.Options
{
    public class Pipeline
    {
        public Receiver[] Receivers { get; set; }
        public Exporter[] Exporters { get; set; }
    }
}