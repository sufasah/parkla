
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Receivers;
public class HttpReceiver
{
    private readonly object _registerLock = new();
    private readonly ILogger<HttpReceiver> _logger;
    private readonly IOptionsMonitor<CollectorOptions> _collectorOptionsMonitor;
    private readonly HttpExportManager _httpExportManager;
    private readonly List<ReceiverPipeline> _receiverPipelines = new();
    private readonly SerialExportManager _serialExportManager;

    public HttpReceiver(
        ILogger<HttpReceiver> logger,
        IOptionsMonitor<CollectorOptions> collectorOptionsMonitor,
        HttpExportManager httpExportManager,
        SerialExportManager serialExportManager
    )
    {
        _logger = logger;
        _collectorOptionsMonitor = collectorOptionsMonitor;
        _httpExportManager = httpExportManager;
        _serialExportManager = serialExportManager;
        
        RegisterOptions();
    }

    private void RegisterOptions() {
        lock(_registerLock) {
            foreach(var pipeline in _collectorOptionsMonitor.CurrentValue.Pipelines) {
                _receiverPipelines.AddRange(pipeline.HttpReceivers.Select(x => new ReceiverPipeline {
                    HttpReceiver = x,
                    Pipeline = pipeline
                }));
            }
        }
    } 

    public async Task ReceiveAsync(HttpContext context) {
        lock(_registerLock) {
            var path = context.Request.Path.Value;
            var receiverPipelines = _receiverPipelines.Where(
                x => String.Compare(
                    x.HttpReceiver.Endpoint, 
                    path, 
                    StringComparison.OrdinalIgnoreCase) == 0
            ).ToArray();
            
            foreach(var receiverPipeline in receiverPipelines) {
                _logger.LogInformation("Executing handler with name '{}' for path '{}'",receiverPipeline.HttpReceiver.Handler.GetType().Name, path);

                try {
                    var handlerResult = receiverPipeline
                        .HttpReceiver
                        .Handler
                        .Handle(ReceiverType.HTTP, new HttpReceiverParam{
                            httpContext = context
                        });

                    if(handlerResult != null)
                        ExportResult(handlerResult, receiverPipeline.Pipeline);
                }
                catch(Exception e) {
                    _logger.LogError(e, "An error occured while handling the request. The result is not generated so not exported.");
                }
            }
        }
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

class ReceiverPipeline {
    public HttpReceiverOptions HttpReceiver { get; set; }
    public PipelineOptions Pipeline { get; set; }
}
