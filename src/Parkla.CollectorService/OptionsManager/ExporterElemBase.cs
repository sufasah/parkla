using System.Globalization;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.OptionsManager;
public abstract class ExporterElemBase
{
    public ExporterBase? ExporterReference { get; set; }
    public static ExporterElemBase GetExporterElem(ExporterType exporterType) {
        var prefix = exporterType.ToString();
        prefix = char.ToUpper(prefix[0],CultureInfo.InvariantCulture) + prefix[1..].ToLower(CultureInfo.InvariantCulture);
        var typeStr = $"{typeof(ReceiverElemBase).Namespace}.{prefix}ExporterElem";
        var type = Type.GetType(typeStr);
        if(type == null)
            throw new TypeUnloadedException(typeStr);

        var instance = Activator.CreateInstance(type)!;
        return (ExporterElemBase)instance;
    }

    public abstract void ConfigureExporter(ExporterOptions exporter, ExporterBase exporterReference);

    public abstract Type GetExporterBaseType();
}