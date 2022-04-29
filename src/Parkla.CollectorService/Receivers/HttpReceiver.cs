using Microsoft.Extensions.Options;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Receivers;
public class HttpReceiver
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
        HttpExporter httpExportManager,
        SerialExporter serialExportManager,
        IOptions<CollectorOptions> options
    )
    {
        _logger = logger;
        _httpExporter = httpExportManager;
        _serialExporter = serialExportManager;
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

    public void Receive(HttpContext context)
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
        
        foreach (var receiverPipeline in receiverPipelines)
        {
            Task.Run(async () =>
            {
                _logger.LogInformation("Executing handler with name '{}' for path '{}'", receiverPipeline.HttpReceiver.Handler.GetType().Name, path);
                try
                {
                    var handlerResult = await receiverPipeline
                        .HttpReceiver
                        .Handler.HandleAsync(ReceiverType.HTTP, new HttpReceiverParam
                        {
                            HttpContext = context,
                            Logger = _logger
                        });

                    if (handlerResult != null)
                    {
                        foreach (var result in handlerResult)
                            ExportResult(result, receiverPipeline.Pipeline);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occured while handling the request with '{}' handler for '{}' path. The result is not generated so not exported.", receiverPipeline.HttpReceiver.Handler.GetType().Name, path);
                }
            });
        }
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

class ReceiverPipeline
{
    public HttpReceiverOptions HttpReceiver { get; set; }
    public PipelineOptions Pipeline { get; set; }

}
