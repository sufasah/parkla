using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Parkla.CollectorService.OptionsManager;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Exporters;
public class HttpExporter : ExporterBase
{
    private readonly HttpClient _client;
    private readonly ILogger<HttpExporter> _logger;
    private readonly JsonSerializerOptions jsonSerializerOptions = new();

    public HttpExporter(
        IHttpClientFactory factory,
        ILogger<HttpExporter> logger
    ) : base(logger)
    {
        _client = factory.CreateClient();
        _logger = logger;

        jsonSerializerOptions.AllowTrailingCommas = true;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        jsonSerializerOptions.MaxDepth = 3;
        jsonSerializerOptions.PropertyNameCaseInsensitive = true;
        jsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    }

    public override async Task ExportAsync(ParkSpaceStatusDto dto, ExporterElemBase exporterElemBase) {
        var exporterElem = (HttpExporterElem) exporterElemBase;
        try {
            var response = await _client.PostAsync(
                exporterElem.Url,
                JsonContent.Create(dto, null, jsonSerializerOptions)
            );
            _logger.LogInformation("HttpExporter [{}]: ParkId='{}', SpaceId='{}', Status='{}' is exported", response.StatusCode == HttpStatusCode.OK ? "OK" : "NOT OK", dto.Parkid, dto.Spaceid, dto.Status);
        } catch(Exception e) {
            _logger.LogError(e, "HttpExporter: ParkId='{}', SpaceId='{}', Status='{}' is not exported", dto.Parkid, dto.Spaceid, dto.Status);
        }
    }

    public override async Task ExportAsync(IEnumerable<ParkSpaceStatusDto> dtos, ExporterElemBase exporterElemBase) {
        if(!dtos.Any()) return;
        var exporterElem = (HttpExporterElem) exporterElemBase;
        
        try {
            var response = await _client.PostAsync(
                exporterElem.Url,
                JsonContent.Create(dtos.ToArray(), null, jsonSerializerOptions)
            );
            
            var str = LogStrList(dtos, "HttpExporter", response.StatusCode == HttpStatusCode.OK);
            _logger.LogInformation(str);
        } catch(Exception e) {
            var str = LogStrList(dtos, "HttpExporter", false);
            _logger.LogError(e,str);
        }

    }
    protected override void DoStart(){}
}