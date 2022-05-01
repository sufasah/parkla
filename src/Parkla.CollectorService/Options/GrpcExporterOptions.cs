namespace Parkla.CollectorService.Options
{
    public class GrpcExporterOptions : ExporterOptions
    {
        public string Group { get; set; }
        public string Address { get; set; }
    }
}