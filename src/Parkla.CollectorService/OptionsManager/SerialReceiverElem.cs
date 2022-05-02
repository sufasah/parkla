using System.IO.Ports;
using System.Reflection;
using Parkla.CollectorService.Generators;
using Parkla.CollectorService.Handlers;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.OptionsManager;
public class SerialReceiverElem : ReceiverElemBase
{
    public readonly static List<Pipe> SerialPipes = new();
    public SerialPort? SerialPort { get; set; }

    public override void ConfigureReceiver(ReceiverOptions receiver, ExporterElemBase[] exporters, Assembly pluginAssembly)
    {
        if (string.IsNullOrWhiteSpace(receiver.PortName))
            throw new ArgumentNullException("PortName", "SerialReceiver PortName configuration value must be given");
        
        Handler = GetHandler(receiver.Handler, typeof(DefaultSerialHandler), pluginAssembly);

        var serialPort = SerialPortPool.GetInstance(receiver.PortName);
        if(serialPort != null) {
            SerialPort = serialPort;
            SerialPipes.Add(new(){
                Receiver = this,
                Exporters = exporters
            });
        }
    }
}