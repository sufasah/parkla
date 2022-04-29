using Microsoft.Extensions.Options;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Receivers;
public class HttpReceiver : ReceiverBase
{
    private readonly object _startLock = new();
    private readonly ILogger<HttpReceiver> _logger;
    private readonly List<ReceiverPipeline> _receiverPipelines = new();
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
                _receiverPipelines.AddRange(pipeline.HttpReceivers.Select(x => new ReceiverPipeline
                {
                    HttpReceiver = x,
                    Pipeline = pipeline
                }));
            }

            Started = true;
            _logger.LogInformation("START: HttpReceiver is started");
        }
    }

    public async Task ReceiveAsync(HttpContext context)
    {
        if (!Started)
            throw new InvalidOperationException("HttpReceiver is not started yet.");

        var path = context.Request.Path.Value;
        var receiverPipelines = _receiverPipelines.Where(
            x => string.Compare(
                x.HttpReceiver.Endpoint,
                path,
                StringComparison.OrdinalIgnoreCase) == 0
        ).ToArray();
        
        var tasks = new List<Task>();
        foreach (var receiverPipeline in receiverPipelines)
        {
            var task = Task.Run(async () =>
            {
                _logger.LogInformation("HttpReceiver: Executing handler with name '{}' for path '{}'", receiverPipeline.HttpReceiver.Handler.GetType().Name, path);
                try
                {
                    var handlerResults = await receiverPipeline
                        .HttpReceiver
                        .Handler.HandleAsync(ReceiverType.HTTP, new HttpReceiverParam
                        {
                            HttpContext = context,
                            Logger = _logger
                        });

                    if (handlerResults != null)
                    {
                        ExportResults(handlerResults, receiverPipeline.Pipeline);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occured while handling the request with '{}' handler for '{}' path. The result is not generated so not exported.", receiverPipeline.HttpReceiver.Handler.GetType().Name, path);
                }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

}

class ReceiverPipeline
{
    public HttpReceiverOptions HttpReceiver { get; set; }
    public PipelineOptions Pipeline { get; set; }

}
