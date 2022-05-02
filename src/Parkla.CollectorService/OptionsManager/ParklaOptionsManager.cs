using System.Reflection;
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.OptionsManager;
public class ParklaOptionsManager {
    public Assembly PluginAssembly { get; set; }
    private readonly ParklaOptions _options;
    private readonly IServiceProvider _provider;

    private bool Configured { get; set; } = false;
    public ParklaOptionsManager(
        IOptions<ParklaOptions> options,
        IServiceProvider provider
    )
    {
        _options = options.Value;
        _provider = provider;
    }

    public void Configure() {
        lock(this) {
            if(Configured) return;

            ConfigurePluginAssembly();
            ConfigurePipelines();

            Configured = true;
        }
    }

    private void ConfigurePluginAssembly() {
        var pluginLibrary = _options.PluginLibrary;
        if (!string.IsNullOrWhiteSpace(pluginLibrary)) {
            var dllFile = new FileInfo(
                pluginLibrary.Contains('/') || pluginLibrary.Contains('\\')
                ? pluginLibrary
                : Path.Combine(".", pluginLibrary)
            );
            PluginAssembly = Assembly.LoadFrom(dllFile.FullName);
        }

    }

    private void ConfigurePipelines() {
        foreach(var pipeline in _options.Pipelines) {
            var exporterElems = new List<ExporterElemBase>();

            foreach (var exporter in pipeline.Exporters)
            {
                var exporterElem = ExporterElemBase.GetExporterElem(exporter.Type);
                var exporterType = exporterElem.GetExporterBaseType();

                if(!exporterType.IsSubclassOf(typeof(ExporterBase)))
                    throw new InvalidCastException("ExporterElem.GetExporterBaseType must return a subclass of ExporterBase");

                var exporterReference = (ExporterBase?)_provider.GetService(exporterType);

                if(exporterReference == null)
                    throw new TypeUnloadedException($"{exporterType.FullName} type exporter was not registered as a service");

                exporterElem.ConfigureExporter(exporter, exporterReference);
                exporterElems.Add(exporterElem);
            }

            foreach (var receiver in pipeline.Receivers)
            {
                var receiverElem = ReceiverElemBase.GetReceiverElem(receiver.Type);
                receiverElem.ConfigureReceiver(receiver, exporterElems.ToArray(), PluginAssembly);
            }

        }

    }
}