using System.Reflection;

namespace Parkla.CollectorService.Options
{
    public class CollectorOptions
    {
        public Assembly pluginAssembly { get; set; }
        public Pipeline[] Pipelines { get; set; }
    }
}