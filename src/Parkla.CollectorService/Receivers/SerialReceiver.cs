using System.IO.Ports;
using Parkla.CollectorService.Exporters;
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
    private readonly List<SerialPort> _serialPorts = new();
    
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
            _serialPorts.ForEach(port => port.Dispose());
            _serialPorts.Clear();

            foreach(var pipeline in options.Pipelines) {
                var serialReceivers = pipeline.SerialReceivers;
                if(serialReceivers.Length == 0) continue;
                
                foreach(var serialReceiver in serialReceivers) {
                    try {
                        var serialPort = new SerialPort(serialReceiver.PortName, 9600);
                        serialPort.Open();
                        serialPort.DataReceived += MakeDataReceived(serialPort, serialReceiver.Handler, pipeline);
                        _serialPorts.Add(serialPort);
                    } catch(Exception e) {
                        _logger.LogError("SerialReceiver: Serial port receiver with {} port name could not be opened. \n{}", serialReceiver.PortName, e.Message);
                    }
                }
            }
        }
    }

    private SerialDataReceivedEventHandler MakeDataReceived(SerialPort serialPort, HandlerBase handler, PipelineOptions pipeline) {
        var param = new SerialReceiverParam {
            SerialPort = serialPort,
            State = null,
            Logger = _logger
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