using System.IO.Ports;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Generators;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.OptionsManager;
public class SerialExporterElem : ExporterElemBase
{
    private readonly Type _exporterBaseType = typeof(SerialExporter); 
    public readonly static List<SerialExporterElem> SerialExporters = new();
    public SerialPort? SerialPort { get; set; }

    public override void ConfigureExporter(ExporterOptions exporter, ExporterBase exporterReference)
    {
        if (string.IsNullOrWhiteSpace(exporter.PortName))
            throw new ArgumentNullException("Url", "SerialExporter PortName value must be given");

        ExporterReference = exporterReference;
        
        var serialPort = SerialPortPool.GetInstance(exporter.PortName);
        if(serialPort != null) {
            SerialPort = serialPort;
            SerialExporters.Add(this);
        }
    }

    public override Type GetExporterBaseType()
    {
        return _exporterBaseType;
    }
}