using Microsoft.AspNetCore.Mvc;
using Parkla.Business.Abstract;
using Parkla.Core.Entities;
using Parkla.Web.Models;

namespace Parkla.Web.Controllers;

public class CitiesController : EntityControllerBase<City>
{
    private readonly ICityService _service;
    public CitiesController(
        ICityService service
    ) : base(service)
    {
        _service = service;
    }

    [HttpGet("")]
    public async Task<List<City>> GetListAsync(
        [FromQuery] PageDto pageDto,
        CancellationToken cancellationToken
    ) {
        return await base.GetPageAsync(pageDto.PageNumber, pageDto.PageSize, cancellationToken);
    }
}