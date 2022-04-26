using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Parkla.Core.DTOs;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Options;
using Parkla.CollectorService.Handlers;
using System.Text.Json;
using System.Net;
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

    public void CatchAllRequests () {
        _httpReceiver.ReceiveAsync(HttpContext);
    }
    
    
}
