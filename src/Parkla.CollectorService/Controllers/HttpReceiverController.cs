using Microsoft.AspNetCore.Mvc;
using Parkla.CollectorService.Receivers;

namespace Parkla.Web.Controllers;

public class HttpReceiverController : ControllerBase
{
    private readonly ILogger<HttpReceiverController> _logger;
    private readonly HttpReceiver _httpReceiver;

    public HttpReceiverController(
        ILogger<HttpReceiverController> logger,
        HttpReceiver httpReceiver )
    {
        _logger = logger;
        _httpReceiver = httpReceiver;
    }

    public async Task CatchAllRequests () {
        await _httpReceiver.ReceiveAsync(HttpContext);
    }
    
    
}
