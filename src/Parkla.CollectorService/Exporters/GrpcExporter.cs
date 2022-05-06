using Google.Protobuf.WellKnownTypes;
using Parkla.CollectorService.OptionsManager;
using Parkla.Core.DTOs;
using Parkla.Protobuf;

namespace Parkla.CollectorService.Exporters;
public class GrpcExporter : ExporterBase
{
    private readonly ILogger<GrpcExporter> _logger;

    public GrpcExporter(
        ILogger<GrpcExporter> logger
    ) : base(logger)
    {
        _logger = logger;
    }
    
    protected override void DoStart() {}

    public override async Task ExportAsync(ParkSpaceStatusDto dto, ExporterElemBase exporterElemBase)
    {
        await ExportAsync(new ParkSpaceStatusDto[]{dto}, exporterElemBase);
    }
    
    public override async Task ExportAsync(IEnumerable<ParkSpaceStatusDto> dtos, ExporterElemBase exporterElemBase)
    {
        if(!dtos.Any()) return;
        var exporterElem = (GrpcExporterElem) exporterElemBase;
        var grpcClient = exporterElem.Client;
        
        if(grpcClient == null) {
            _logger.LogWarning("GrpcExporter: GrpcClient could not found with client to export");
            return;
        }

        var data = new Data();
        string logStr;
        
        data.Group = exporterElem.Group;
        foreach (var dto in dtos)
        {
            data.DataList.Add(Any.Pack(
                new ParkSpaceStatus {
                    Parkid = dto.ParkId.ToString(),
                    Spaceid = dto.SpaceId,
                    Status = (int)dto.Status,
                    DateTime = Timestamp.FromDateTime(dto.DateTime.ToUniversalTime())
                }
            ));
        }

        try {
            await grpcClient!.ReceiveAsync(data);
            logStr = LogStrList(dtos, "GrpcExporter", true);
            _logger.LogInformation(logStr);
        }
        catch(Exception e) {
            logStr = LogStrList(dtos, "GrpcExporter", false);
            _logger.LogError(e,logStr);
        }
    }

}