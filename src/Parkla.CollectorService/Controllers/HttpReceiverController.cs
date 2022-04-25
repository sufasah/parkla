using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Parkla.Core.DTOs;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Handlers;
using Parkla.CollectorService.Options;

namespace Parkla.Web.Controllers;

public class HttpReceiverController : ControllerBase
{
    private readonly IOptionsMonitor<CollectorOptions> _options;
    private readonly HttpClient _client;
    private readonly ExportManager _exportManager;
    
    public HttpReceiverController(
        IOptionsMonitor<CollectorOptions> options,
        IHttpClientFactory httpClientFactory,
        ExportManager exportManager)
    {
        _options = options;
        _client = httpClientFactory.CreateClient();
        _exportManager = exportManager;
    }

    public async Task Receive () {
        var path = HttpContext.Request.Path.Value;
        var pipelines = _options.CurrentValue.Pipelines;

        foreach(var pipeline in pipelines) {
            var httpReceivers = pipeline.Receivers.Where(x => {
                var isHttpReceiver = x.Type == ReceiverType.HTTP;
                if(!isHttpReceiver) return false;
                return string.Compare(
                    ((HttpReceiver)x).Endpoint,
                    path,
                    StringComparison.OrdinalIgnoreCase
                ) == 0;
            }).ToArray();
            if(httpReceivers.Length == 0) continue;

            var exporters = pipeline.Exporters;
            var handlerResults = ExecuteHandlers(httpReceivers);
            foreach(var exporter in exporters) {
                _exportManager.Export(exporter, handlerResults);
            }
        }
    }
    
    private List<ParkSpaceStatusDto> ExecuteHandlers(Receiver[] httpReceivers) {
        var results = new List<ParkSpaceStatusDto>();
        
        foreach(HttpReceiver httpReceiver in httpReceivers) {
           
            var handler = (HandlerBase)HandlerBase.GetInstance(httpReceiver.Handler);
            var handlerResult = handler.Handle(new HttpReceiverParams{
                httpContext = HttpContext,
            });
            results.Add(handlerResult);
        }
        
        return results;
    }
    
}
