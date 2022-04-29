using System.Text.Json;
using Parkla.Core.DTOs;
using Parkla.CollectorService.Library;
using System.Text.Json.Serialization;

namespace Parkla.CollectorService.Handlers;
public class DefaultHttpHandler : HandlerBase
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new();
    public DefaultHttpHandler()
    {
        jsonSerializerOptions.AllowTrailingCommas = true;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        jsonSerializerOptions.MaxDepth = 2;
        jsonSerializerOptions.PropertyNameCaseInsensitive = true;
        jsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    }
    // THIS HANDLE METHOD WILL BE CALLED WHEN A REQUEST IS SENT
    public override async Task<IEnumerable<ParkSpaceStatusDto>> HandleAsync(ReceiverType receiverType, object param)
    {
        if(receiverType != ReceiverType.HTTP)
            throw new ArgumentException("DefaultHttpHandler only handles http requests");

        var httpParam = (HttpReceiverParam) param;
        var httpContext = httpParam.HttpContext;
        var logger = httpParam.Logger;
        var request = httpContext.Request;

        IEnumerable<ParkSpaceStatusDto> results;
        try {
            var jsonElement = await request.ReadFromJsonAsync<JsonElement>();

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
            logger.LogInformation("DefaultHttpHandler: Request body could not be deserialized as json\n{}", e.ToString());
            return Array.Empty<ParkSpaceStatusDto>();
        }
        
        return results;
    }

}