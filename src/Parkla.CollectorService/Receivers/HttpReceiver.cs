using System.Collections.ObjectModel;
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.Receivers;
public class HttpReceiver : ReceiverBase
{
    private readonly object _startLock = new();
    private readonly ILogger<HttpReceiver> _logger;
    private readonly List<HttpPipelines> _httpPipelinesList = new();
    private readonly HttpExporter _httpExporter;
    private readonly SerialExporter _serialExporter;
    private readonly IOptions<CollectorOptions> _options;
    private bool Started { get; set; } = false;

    public HttpReceiver(
        ILogger<HttpReceiver> logger,
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
                _logger.LogWarning("HttpReceiver.Start: HttpReceiver is already started.");
                return;
            }

            foreach (var pipeline in _options.Value.Pipelines)
            {
                var httpReceivers = pipeline.HttpReceivers;
                if (httpReceivers.Length == 0) continue;

                foreach(var httpReceiver in httpReceivers) {
                    var httpPipelines = _httpPipelinesList.Find(x => 
                        string.Compare(
                            x.Endpoint, 
                            httpReceiver.Endpoint, 
                            StringComparison.OrdinalIgnoreCase
                        ) == 0
                    );
                    
                    if(httpPipelines == null) {
                        httpPipelines = new(){
                            Endpoint = httpReceiver.Endpoint,
                            Pipelines = new()
                        };
                        _httpPipelinesList.Add(httpPipelines);
                    }

                    httpPipelines.Pipelines.Add(new() {
                        Handler = httpReceiver.Handler,
                        HttpExporters = pipeline.HttpExporters,
                        SerialExporters = pipeline.SerialExporters
                    });
                }
            }

            Started = true;
            _logger.LogInformation("START: HttpReceiver is started");
        }
    }

    public async Task ReceiveAsync(HttpContext context, HttpPipelines httpPipelines)
    {
        if (!Started)
            throw new InvalidOperationException("HttpReceiver is not started yet.");
        
        var tasks = new List<Task>();
        foreach (var httpPipeline in httpPipelines.Pipelines)
        {
            var handler = httpPipeline.Handler;
            var task = Task.Run(async () =>
            {
                _logger.LogInformation("HttpReceiver: Executing handler with name '{}' for path '{}'", handler.GetType().Name, httpPipelines.Endpoint);
                try
                {
                    var handlerResults = await handler.HandleAsync(ReceiverType.HTTP, new HttpReceiverParam {
                            HttpContext = context,
                            Logger = _logger
                        }).ConfigureAwait(false);

                    if (handlerResults != null)
                    {
                        ExportResults(handlerResults, httpPipeline.HttpExporters, httpPipeline.SerialExporters);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occured while handling the request with '{}' handler for '{}' path. The result is not generated so not exported.", handler.GetType().Name, httpPipelines.Endpoint);
                }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }

    public ReadOnlyCollection<HttpPipelines> GetHttpPipelinesList() {
        return _httpPipelinesList.AsReadOnly();
    }
}

public class HttpPipelines
{
    public string Endpoint { get; set; }
    public List<HttpPipeline> Pipelines { get; set; }
}

public class HttpPipeline {
    public HandlerBase Handler { get; set; }
    public HttpExporterOptions[] HttpExporters { get; set; }
    public SerialExporterOptions[] SerialExporters { get; set; }
}
