using Collector;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.Receivers;
public class GrpcReceiver : ReceiverBase
{
    private readonly object _startLock = new();
    private readonly ILogger<GrpcReceiver> _logger;
    private readonly HttpExporter _httpExporter;
    private readonly SerialExporter _serialExporter;
    private readonly IOptions<CollectorOptions> _options;
    private readonly List<GrpcPipeline> _grpcPipelines = new();

    private bool Started { get; set; } = false;

    public GrpcReceiver(
        ILogger<GrpcReceiver> logger,
        HttpExporter httpExporter,
        SerialExporter serialExporter,
        GrpcExporter grpcExporter,
        IOptions<CollectorOptions> options
    ) : base(logger, httpExporter, serialExporter, grpcExporter)
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
                _logger.LogWarning("HttpReceiver.Start: HttpReceiver is already started.");
                return;
            }

            foreach (var pipeline in _options.Value.Pipelines)
            {
                if(pipeline.GrpcReceivers.Length == 0) 
                    continue;
                
                foreach(var grpcReceiver in pipeline.GrpcReceivers) {
                    _grpcPipelines.Add(new() {
                        Handler = grpcReceiver.Handler,
                        SerialExporters = pipeline.SerialExporters,
                        GrpcExporters = pipeline.GrpcExporters,
                        HttpExporters = pipeline.HttpExporters
                    });
                }
            }

            Started = true;
            _logger.LogInformation("START: HttpReceiver is started");
        }
    }

    public async Task ReceiveAsync(Data data, ServerCallContext context) {
        var tasks = new List<Task>();
        foreach(var pipeline in _grpcPipelines) {
            var handler = pipeline.Handler;
            var httpExporters = pipeline.HttpExporters;
            var serialExporters = pipeline.SerialExporters;
            var grpcExporters = pipeline.GrpcExporters;

            var task = Task.Run(async () => {
                _logger.LogInformation("GrpcReceiver: Executing handler with name '{}'", handler.GetType().Name);
                try {
                    var handlerResults = await handler.HandleAsync(ReceiverType.GRPC, new GrpcReceiverParam {
                        Data = data,
                        Context = context,
                        Logger = _logger
                    });

                    if (handlerResults != null) {
                        ExportResults(handlerResults, httpExporters, serialExporters, grpcExporters);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occured while handling the request '{}' handler. The result is not generated so it will not be exported.", handler.GetType().Name);
                }
            });
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);
    }
}

public class GrpcPipelines {
    public List<GrpcPipeline> Pipelines { get; set; }
}

public class GrpcPipeline {
    public HandlerBase Handler { get; set; }
    public HttpExporterOptions[] HttpExporters { get; set; }
    public SerialExporterOptions[] SerialExporters { get; set; }
    public GrpcExporterOptions[] GrpcExporters { get; set; }
}