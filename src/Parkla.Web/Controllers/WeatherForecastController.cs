using Microsoft.AspNetCore.Mvc;

namespace Parkla.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{

    private readonly ILogger<WeatherForecastController> _logger;
    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
    }

    [HttpPost("/test")]
    public object Test(object any) {
        _logger.LogInformation(any.ToString());
        return any;
    }
    
}
