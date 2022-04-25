using Microsoft.AspNetCore.Http;

namespace Parkla.CollectorService.Library;
public class HttpReceiverParam : ParamBase
{
    public HttpContext httpContext { get; set; }
}