using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Enums;
using Parkla.CollectorService.Handlers;
using Parkla.CollectorService.Options;

namespace Parkla.Web.Controllers;

public class HttpReceiverController : ControllerBase
{

    private readonly ILogger<HttpReceiverController> _logger;
    private readonly IOptionsMonitor<CollectorOptions> _options;
    private readonly HttpClient _client;
    
    public HttpReceiverController(
        ILogger<HttpReceiverController> logger,
        IOptionsMonitor<CollectorOptions> options,
        IHttpClientFactory httpClientFactory
    )
    {
        _logger = logger;
        _options = options;
        _client = httpClientFactory.CreateClient();
    }
    public async Task Receive () {
        /*var exportUrls = _options.CurrentValue.Export.Http.Urls;
        foreach(var url in exportUrls) {
            var response = await HttpClientJsonExtensions.PostAsJsonAsync(_client, url+"/test", dto);
            _logger.LogInformation("{0}: ParkSpaceStatus with {1} Parkid is sent. Response status {2}", url, dto.Parkid, response.StatusCode);
        }*/
        var path = HttpContext.Request.Path.Value;
        var pipelines = _options.CurrentValue.Pipelines;
        var defaultHttpReceiverHandler = new DefaultHttpReceiverHandler();
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

            List<object> handlerResults;
            foreach( HttpReceiver httpReceiver in httpReceivers) {
                object handlerResult;
                if(string.Compare(httpReceiver.Handler,"default",StringComparison.OrdinalIgnoreCase) == 0) {
                    handlerResult = defaultHttpReceiverHandler.Handle(new HttpReceiverParams{

                    });
                }
                else {

                }
            }

        }
    }
    
}
