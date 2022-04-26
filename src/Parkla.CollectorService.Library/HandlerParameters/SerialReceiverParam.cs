using System.IO.Ports;

namespace Parkla.CollectorService.Library;
public class SerialReceiverParam : ParamBase
{
    public SerialPort SerialPort { get; set; }
    public SerialDataReceivedEventArgs SerialDataReceivedEventArgs { get; set; }
    public object State { get; set; }
}