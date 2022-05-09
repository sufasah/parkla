using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Parkla.Core.Options;

namespace Parkla.Web.Controllers;
[ApiController]
[Route(WebOptions.API_PREFIX+"/[controller]")]
[Authorize]
public class ApiControllerBase: ControllerBase
{
}
