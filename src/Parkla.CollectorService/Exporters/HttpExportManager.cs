
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Exporters;
public class HttpExportManager
{
    private readonly HttpClient _client;

    public HttpExportManager(IHttpClientFactory factory)
    {
        _client = factory.CreateClient();
    }

    public async Task ExportAsync(ParkSpaceStatusDto dto, Uri url) {
        var response = await _client.PostAsync(url,JsonContent.Create(dto));
    }
}