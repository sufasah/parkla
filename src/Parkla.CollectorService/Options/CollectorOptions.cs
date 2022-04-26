using System.Reflection;

namespace Parkla.CollectorService.Options
{
    public class CollectorOptions
    {
        public Assembly pluginAssembly { get; set; }
        public PipelineOptions[] Pipelines { get; set; }
    }
}