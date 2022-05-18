using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Parkla.Business.Abstract;
using Parkla.Core.DTOs;
using Parkla.Core.Enums;
using Parkla.Protobuf;
using static Parkla.Protobuf.Collector;

namespace Collector;
public class CollectorService : CollectorBase
{
    private readonly ILogger<CollectorService> _logger;
    private readonly ICollectorService _collectorService;

    public CollectorService(
        ILogger<CollectorService> logger,
        ICollectorService collectorService
    ) {
        _logger = logger;
        _collectorService = collectorService;
    }
    public override Task<Empty> Receive(Data data, ServerCallContext context)
    {
        List<ParkSpaceStatusDto> result = new();
        foreach (var item in data.DataList) {
            try {
                var status = item.Unpack<ParkSpaceStatus>();
                var parkSpaceStatusDto= new ParkSpaceStatusDto() {
                    ParkId = Guid.Parse(status.ParkId),
                    SpaceId = status.SpaceId,
                    Status = (SpaceStatus)status.Status,
                    DateTime = status.DateTime.ToDateTime()
                };
                result.Add(parkSpaceStatusDto);
                _logger.LogInformation(
                    "GrpcReceiver: ParkId='{}' SpaceId='{}' Status='{}' DateTime='{}' is received", 
                    parkSpaceStatusDto.ParkId,
                    parkSpaceStatusDto.SpaceId,
                    parkSpaceStatusDto.Status,
                    parkSpaceStatusDto.DateTime
                );
            }
            catch (Exception e) {
                _logger.LogInformation("GrpcReceiver: Grpc message could not be deserialized\n{}", e.ToString());
            }
        }
        _collectorService.CollectParkSpaceStatusAsync(result);
        return Task.FromResult(new Empty());
    }
}