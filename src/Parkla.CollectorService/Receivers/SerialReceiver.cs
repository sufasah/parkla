using System.IO.Ports;
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Receivers;
public class SerialReceiver : ReceiverBase
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
    ) : base(logger, httpExporter, serialExporter)
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
                        var found = _serialPorts.FirstOrDefault(x => x.PortName == serialReceiver.PortName);
                        if(found != null)
                            _logger.LogError("SerialReceiver: Serial port with {} port name already in use", serialReceiver.PortName);
                        _logger.LogError(e, "SerialReceiver: Serial port receiver with {} port name could not be opened. \n", serialReceiver.PortName);
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

        return (object sender, SerialDataReceivedEventArgs args) =>
        {
            _logger.LogInformation("SerialReceiver: Executing handler with name '{}'", handler.GetType().Name);
            try
            {
                param.SerialDataReceivedEventArgs = args;
                var handlerResults = ResultSafe(handler.HandleAsync(ReceiverType.SERIAL, param)).Result;

                if (handlerResults != null)
                {
                   ExportResults(handlerResults, pipeline);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while handling the request '{}' handler. The result is not generated so it will not be exported.", handler.GetType().Name);
            }
        };
    }

    private static async Task<IEnumerable<ParkSpaceStatusDto>> ResultSafe(Task<IEnumerable<ParkSpaceStatusDto>> task) {
        // For legacy synchronization context deadlocks configureAwait
        var taskResult = await task.ConfigureAwait(false);
        return taskResult;
    }
}