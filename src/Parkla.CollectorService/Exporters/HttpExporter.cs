using System.Net;
using System.Text;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Exporters;
public class HttpExporter
{
    private readonly HttpClient _client;
    private readonly ILogger<HttpExporter> _logger;

    public HttpExporter(
        IHttpClientFactory factory,
        ILogger<HttpExporter> logger
    )
    {
        _client = factory.CreateClient();
        _logger = logger;
    }

    public async Task ExportAsync(ParkSpaceStatusDto dto, Uri url) {
        try {
            var response = await _client.PostAsync(
                url,
                JsonContent.Create(dto)
            );
            _logger.LogInformation("HttpExporter [{}]: ParkId='{}', SpaceId='{}', Status='{}' is exported", response.StatusCode == HttpStatusCode.OK ? "OK" : "NOT OK", dto.Parkid, dto.Spaceid, dto.Status);
        } catch(Exception e) {
            _logger.LogError(e, "HttpExporter: ParkId='{}', SpaceId='{}', Status='{}' is not exported", dto.Parkid, dto.Spaceid, dto.Status);
        }
    }

    public async Task ExportAsync(IEnumerable<ParkSpaceStatusDto> dtos, Uri url) {
        if(!dtos.Any()) return;

        try {
            var response = await _client.PostAsync(
                url,
                JsonContent.Create(dtos.ToArray())
            );
            
            var str = LogStrList(dtos, "HttpExporter", response.StatusCode == HttpStatusCode.OK);
            _logger.LogInformation(str);
        } catch(Exception e) {
            var str = LogStrList(dtos, "HttpExporter", false);
            _logger.LogError(e,str);
        }

    }

    public static string LogStrList(IEnumerable<ParkSpaceStatusDto> dtos, string className, bool successful) {           
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();
        foreach(var dto in dtos) {
            stringBuilder.AppendFormat("{0} [{1}]: ParkId='{2}', SpaceId='{3}', Status='{4}' is{5} exported\n", 
                className, 
                successful ? "SUCCESS" : "FAIL", 
                dto.Parkid, 
                dto.Spaceid, 
                dto.Status, 
                successful ? "" : " not");
        }
        return stringBuilder.ToString();
    }
}