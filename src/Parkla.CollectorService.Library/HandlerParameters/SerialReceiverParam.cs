using System.IO.Ports;
using Microsoft.Extensions.Logging;

namespace Parkla.CollectorService.Library;
public class SerialReceiverParam : ParamBase
{
    public SerialPort SerialPort { get; set; }
    public SerialDataReceivedEventArgs SerialDataReceivedEventArgs { get; set; }
    public object? State { get; set; }
    public ILogger Logger { get; set; }
}