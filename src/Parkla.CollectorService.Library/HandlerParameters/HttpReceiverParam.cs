using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Parkla.CollectorService.Library;
public class HttpReceiverParam : ParamBase
{
    public HttpContext HttpContext { get; set; }
    public ILogger Logger { get; set; }
}