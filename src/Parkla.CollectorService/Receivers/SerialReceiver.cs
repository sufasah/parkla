using System.IO.Ports;
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Receivers;
public class SerialReceiver : ReceiverBase, IDisposable
{
    private readonly ILogger<SerialReceiver> _logger;
    private readonly HttpExporter _httpExporter;
    private readonly SerialExporter _serialExporter;
    private readonly IOptions<CollectorOptions> _options;
    private readonly List<SerialPipelines> _serialPipelinesList = new();
    private readonly object _startLock = new();
    private bool Started { get; set; } = false;
    private bool disposed = false;
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
                    var serialPipelines = _serialPipelinesList.Find(x => x.SerialPort.PortName == serialReceiver.PortName);
                    
                    if(serialPipelines == null) {
                        try {
                            var serialPort = new SerialPort(serialReceiver.PortName, 9600);
                            serialPort.Open();
                            serialPipelines = new() {
                                SerialPort = serialPort,
                                Pipelines = new()
                            };
                            serialPort.DataReceived += MakeDataReceived(serialPort, serialPipelines);
                            _serialPipelinesList.Add(serialPipelines);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e, "SerialReceiver: Serial port receiver with {} port name could not be opened. \n", serialReceiver.PortName);
                            //if port name will be available other receivers with same serial port can work but this one's handler won't exist 
                            continue;
                        }
                    }

                    serialPipelines.Pipelines.Add(new() {
                        Handler = serialReceiver.Handler,
                        HttpExporters = pipeline.HttpExporters,
                        SerialExporters = pipeline.SerialExporters
                    });   
                }
            }

            Started = true;
            _logger.LogInformation("START: SerialReceiver is started");
        }
    }

    private SerialDataReceivedEventHandler MakeDataReceived(SerialPort serialPort, SerialPipelines serialPipelines)
    {
        var param = new SerialReceiverParam
        {
            SerialPort = serialPort,
            State = null,
            Logger = _logger
        };

        return (object sender, SerialDataReceivedEventArgs args) =>
        {
            foreach(var pipeline in serialPipelines.Pipelines) {
                var handler = pipeline.Handler;
                // _logger.LogInformation("SerialReceiver: Executing handler with name '{}'", handler.GetType().Name);
                try {
                    param.SerialDataReceivedEventArgs = args;
                    var handlerResults = ResultSafe(handler.HandleAsync(ReceiverType.SERIAL, param)).Result;

                    if (handlerResults != null) {
                        ExportResults(handlerResults, pipeline.HttpExporters, pipeline.SerialExporters);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occured while handling the request '{}' handler. The result is not generated so it will not be exported.", handler.GetType().Name);
                }
            }
        };
    }

    private static async Task<IEnumerable<ParkSpaceStatusDto>> ResultSafe(Task<IEnumerable<ParkSpaceStatusDto>> task) {
        // For legacy synchronization context deadlocks configureAwait
        var taskResult = await task.ConfigureAwait(false);
        return taskResult;
    }

    public new void Dispose(bool disposing) {
        if(disposed) {
            return;
        }

        if(disposing) {
            base.Dispose();
            _httpExporter.Dispose();
            _serialExporter.Dispose();
            foreach(var item in _serialPipelinesList) {
                item.SerialPort.Dispose();
            }
        }

        disposed = true;
    }

    public new void Dispose() {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~SerialReceiver() {
        Dispose(false);
    }
}

class SerialPipelines {
    public SerialPort SerialPort { get; set; }
    public List<SerialPipeline> Pipelines { get; set; }
}

class SerialPipeline {
    public HandlerBase Handler { get; set; }
    public IEnumerable<HttpExporterOptions> HttpExporters { get; set; }
    public IEnumerable<SerialExporterOptions> SerialExporters { get; set; }
}