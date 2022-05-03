using System.Text.Json;
using System.Text.Json.Serialization;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IValidator<ParkSpaceStatusDto> _parkSpaceStatusValidator;

    public CollectorController(
        ILogger<CollectorController> logger,
        IValidator<ParkSpaceStatusDto> ParkSpaceStatusValidator
    )
    {
        _logger = logger;
        _parkSpaceStatusValidator = ParkSpaceStatusValidator;
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
            var validationResult = _parkSpaceStatusValidator.Validate(dto);
            //if(validationResult.IsValid) {
                _logger.LogInformation(
                    "HttpReceiver: ParkId='{}' SpaceId='{}' Status='{}' DateTime='{}' is received", 
                    dto.Parkid,
                    dto.Spaceid,
                    dto.Status,
                    dto.DateTime
                );
            //}
        }
    }
    
}
