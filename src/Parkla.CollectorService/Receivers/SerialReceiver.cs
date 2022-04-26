using System.IO.Ports;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Handlers;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Receivers;
public class SerialReceiver
{
    private readonly object registeringLock = new();
    private readonly ILogger<SerialReceiver> _logger;
    private readonly HttpExportManager _httpExportManager;
    private readonly SerialExportManager _serialExportManager;
    private readonly List<SerialPortPipeline> _serialPortPipelines = new();
    
    public SerialReceiver(
        ILogger<SerialReceiver> logger,
        HttpExportManager httpExportManager,
        SerialExportManager serialExportManager
    )
    {
        _logger = logger;
        _httpExportManager = httpExportManager;
        _serialExportManager = serialExportManager;
    }
    public void RegisterOpitons(CollectorOptions options) {
        lock(registeringLock) {
            _serialPortPipelines.Clear();

            foreach(var pipeline in options.Pipelines) {
                var serialReceivers = pipeline.SerialReceivers;
                if(serialReceivers.Length == 0) continue;
                
                foreach(var serialReceiver in serialReceivers) {
                    var serialPort = new SerialPort(serialReceiver.PortName, 9600);
                    serialPort.DataReceived += MakeDataReceived(serialPort, serialReceiver.Handler, pipeline);
                    
                    /*_serialPortPipelines.Add(new SerialPortPipeline {
                        SerialPort = serialPort,
                        Pipeline = pipeline
                    });*/
                }
            }
        }
    }

    private SerialDataReceivedEventHandler MakeDataReceived(SerialPort serialPort, HandlerBase handler, PipelineOptions pipeline) {
        var param = new SerialReceiverParam {
            SerialPort = serialPort,
            State = null,
        };

        return (object sender, SerialDataReceivedEventArgs args) => {
            try {
                param.SerialDataReceivedEventArgs = args;
                var handlerResult = handler.Handle(ReceiverType.SERIAL, param);

                if(handlerResult != null) {
                    foreach(var result in handlerResult)
                        ExportResult(result, pipeline);
                }
            }
            catch(Exception e) {
                _logger.LogError(e, "An error occured while handling the request. The result is not generated so it will not be exported.");
            }
        };
    }

    private void ExportResult(ParkSpaceStatusDto result, PipelineOptions pipeline) {
        _logger.LogInformation("ParkId='{}', SpaceId='{}', Status='{}' is exporting with all exporters in pipeline", result.Parkid, result.Spaceid, result.Status);

        foreach(var exporter in pipeline.HttpExporters) {
            _httpExportManager.ExportAsync(result, exporter.Url);
        }

        foreach(var exporter in pipeline.SerialExporters) {
            _serialExportManager.Enqueue(result, exporter.PortName);
        }
    }

}

class SerialPortPipeline {
    public SerialPort SerialPort { get; set; }
    public PipelineOptions Pipeline { get; set; }
}