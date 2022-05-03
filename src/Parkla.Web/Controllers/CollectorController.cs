using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Parkla.Business.Abstract;
using Parkla.Core.DTOs;

namespace Parkla.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class CollectorController : ControllerBase
{
    private static readonly JsonSerializerOptions jsonSerializerOptions = new(){
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        MaxDepth = 3,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };
    private readonly ILogger<CollectorController> _logger;
    private readonly ICollectorService _collectorService;
    public CollectorController(
        ILogger<CollectorController> logger,
        ICollectorService collectorService
    )
    {
        _logger = logger;
        _collectorService = collectorService;
    }

    [HttpPost("receive")]
    public async Task Receive() {
        IEnumerable<ParkSpaceStatusDto> results;
        try {
            var jsonElement = await Request.ReadFromJsonAsync<JsonElement>();

            if(jsonElement.ValueKind == JsonValueKind.Array) {
                results = jsonElement.Deserialize<ParkSpaceStatusDto[]>(jsonSerializerOptions)
                    ?? Array.Empty<ParkSpaceStatusDto>();
            }
            else if(jsonElement.ValueKind == JsonValueKind.Object) {
                var obj = jsonElement.Deserialize<ParkSpaceStatusDto>(jsonSerializerOptions);
                results = obj == null 
                    ? Array.Empty<ParkSpaceStatusDto>()
                    : new ParkSpaceStatusDto[]{obj};
            }
            else throw new InvalidDataException("Data must be an object or array of objects");
        }
        catch (Exception e) {
            _logger.LogInformation("HttpReceiver: Request body could not be deserialized as json\n{}", e.ToString());
            results = Array.Empty<ParkSpaceStatusDto>();
        }

        foreach (var dto in results)
        {
            _logger.LogInformation(
                "HttpReceiver: ParkId='{}' SpaceId='{}' Status='{}' DateTime='{}' is received", 
                dto.Parkid,
                dto.Spaceid,
                dto.Status,
                dto.DateTime
            );
        }

        _collectorService.CollectParkSpaceStatus(results);
    }
    
}
