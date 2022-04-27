
using System.Net;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Exporters;
public class HttpExportManager
{
    private readonly HttpClient _client;
    private readonly ILogger<HttpExportManager> _logger;

    public HttpExportManager(
        IHttpClientFactory factory,
        ILogger<HttpExportManager> logger
    )
    {
        _client = factory.CreateClient();
        _logger = logger;
    }

    public async Task ExportAsync(ParkSpaceStatusDto dto, Uri url) {
        try {
            var response = await _client.PostAsync(url,JsonContent.Create(dto));

            if(response.StatusCode == HttpStatusCode.OK)
                _logger.LogInformation("HttpExporter [OK]: ParkId='{}', SpaceId='{}', Status='{}' is exported", dto.Parkid, dto.Spaceid, dto.Status);
            else
                _logger.LogInformation("HttpExporter [NOT OK]: ParkId='{}', SpaceId='{}', Status='{}' is not exported", dto.Parkid, dto.Spaceid, dto.Status);
        } catch(Exception e) {
            _logger.LogError(e, "HttpExporter: ParkId='{}', SpaceId='{}', Status='{}' is not exported", dto.Parkid, dto.Spaceid, dto.Status);
        }
    }

    public async Task ExportAsync(IEnumerable<ParkSpaceStatusDto> dtos, Uri url) {
        foreach(var dto in dtos)
            await ExportAsync(dto, url);
    }
}