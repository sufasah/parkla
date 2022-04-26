using System.IO.Ports;
using Parkla.CollectorService.Handlers;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.Receivers;
public class SerialReceiver
{
    private readonly object registeringLock = new();
    private readonly ILogger<SerialReceiver> _logger;
    private readonly List<HttpExporterOptions> httpExporters = new(); 
    private readonly List<SerialExporterOptions> serialExporters = new(); 
    private List<SerialPort> ReceivePorts { get; set; }
    
    public SerialReceiver(
        ILogger<SerialReceiver> logger
    )
    {
        _logger = logger;
    }
    public void RegisterOpitons(CollectorOptions options) {
        lock(registeringLock) {
            foreach(var pipeline in options.Pipelines) {
                var serialReceivers = pipeline.SerialReceivers;
                if(serialReceivers.Length == 0) continue;
                
                foreach(var serialReceiver in serialReceivers) {
                    var serialPort = new SerialPort(serialReceiver.PortName, 9600);
                    serialPort.DataReceived += MakeDataReceived(serialPort, serialReceiver.Handler);
                }
            }
        }
    }

    private SerialDataReceivedEventHandler MakeDataReceived(SerialPort serialPort, HandlerBase handler) {
        return (object sender, SerialDataReceivedEventArgs args) => {
            try {
                var handlerResult = handler.Handle(ReceiverType.SERIAL, new SerialReceiverParam {
                    SerialPort = serialPort,
                    SerialDataReceivedEventArgs = args
                });

                if(handlerResult != null) {
                    ExportResult(handlerResult);
                }
            }
            catch(Exception e) {
                _logger.LogError(e, "An error occured while handling the request. The result is not generated so it will not be exported.");
            }
        };
    }

    private ExportResult()
}