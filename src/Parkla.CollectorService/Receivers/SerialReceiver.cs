using System.IO.Ports;
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Receivers;
public class SerialReceiver
{
    private readonly ILogger<SerialReceiver> _logger;
    private readonly HttpExporter _httpExporter;
    private readonly SerialExporter _serialExporter;
    private readonly IOptions<CollectorOptions> _options;
    private readonly List<SerialPort> _serialPorts = new();
    private readonly object _startLock = new();
    private bool Started { get; set; } = false;
    public SerialReceiver(
        ILogger<SerialReceiver> logger,
        HttpExporter httpExporter,
        SerialExporter serialExporter,
        IOptions<CollectorOptions> options
    )
    {
        _logger = logger;
        _httpExporter = httpExporter;
        _serialExporter = serialExporter;
        _options = options;
    }
    public void Start()
    {
        lock (_startLock)
        {
            if (Started)
            {
                _logger.LogWarning("SerialReceiver.StartAsnyc: SerialReceiver is already started.");
                return;
            }

            foreach (var pipeline in _options.Value.Pipelines)
            {
                var serialReceivers = pipeline.SerialReceivers;
                if (serialReceivers.Length == 0) continue;

                foreach (var serialReceiver in serialReceivers)
                {
                    try
                    {
                        var serialPort = new SerialPort(serialReceiver.PortName, 9600);
                        serialPort.Open();
                        serialPort.DataReceived += MakeDataReceived(serialPort, serialReceiver.Handler, pipeline);
                        _serialPorts.Add(serialPort);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("SerialReceiver: Serial port receiver with {} port name could not be opened. \n{}", serialReceiver.PortName, e.Message);
                    }
                }
            }

            Started = true;
            _logger.LogInformation("START: SerialReceiver is started");
        }
    }

    private SerialDataReceivedEventHandler MakeDataReceived(SerialPort serialPort, HandlerBase handler, PipelineOptions pipeline)
    {
        var param = new SerialReceiverParam
        {
            SerialPort = serialPort,
            State = null,
            Logger = _logger
        };

        return async (object sender, SerialDataReceivedEventArgs args) =>
        {
            try
            {
                param.SerialDataReceivedEventArgs = args;
                var handlerResult = await handler.HandleAsync(ReceiverType.SERIAL, param);

                if (handlerResult != null)
                {
                    foreach (var result in handlerResult)
                        ExportResult(result, pipeline);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while handling the request '{}' handler. The result is not generated so it will not be exported.", handler.GetType().Name);
            }
        };
    }

    private void ExportResult(ParkSpaceStatusDto result, PipelineOptions pipeline)
    {
        _logger.LogInformation("ParkId='{}', SpaceId='{}', Status='{}' is exporting with all exporters in the pipeline", result.Parkid, result.Spaceid, result.Status);

        foreach (var exporter in pipeline.HttpExporters)
        {
            _httpExporter.ExportAsync(result, exporter.Url);
        }

        foreach (var exporter in pipeline.SerialExporters)
        {
            _serialExporter.Enqueue(result, exporter.PortName);
        }
    }

}