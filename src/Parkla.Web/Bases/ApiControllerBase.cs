using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parkla.Core.Options;

namespace Parkla.Web.Controllers;
[ApiController]
[Route(WebOptions.API_PREFIX+"/[controller]")]
[Authorize]
[Produces("application/json")]
public class ApiControllerBase: ControllerBase
{
}
