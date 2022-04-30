
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
        try {
            var response = await _client.PostAsync(
                url,
                JsonContent.Create(dtos)
            );
            
            var str = LogStrList(dtos, response.StatusCode == HttpStatusCode.OK);
            _logger.LogInformation(str);
        } catch(Exception e) {
            var str = LogStrList(dtos, false);
            _logger.LogError(e,str);
        }

    }

    private static string LogStrList(IEnumerable<ParkSpaceStatusDto> dtos, bool successful) {
        var stringBuilder = new StringBuilder();
        stringBuilder.AppendLine();
        foreach(var dto in dtos) {
            stringBuilder.AppendFormat("HttpExporter [{}]: ParkId='{}', SpaceId='{}', Status='{}' is exported\n", successful ? "SUCCESS" : "FAIL", dto.Parkid, dto.Spaceid, dto.Status);
        }

        return stringBuilder.ToString();
    }
}