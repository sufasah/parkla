using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Parkla.Core.DTOs;
using Parkla.Core.Enums;
using Parkla.Protobuf;
using static Parkla.Protobuf.Collector;

namespace Collector;
public class CollectorService : CollectorBase
{
    private readonly ILogger<CollectorService> _logger;

    public CollectorService(ILogger<CollectorService> logger) {
        _logger = logger;
    }
    public override Task<Empty> Receive(Data data, ServerCallContext context)
    {
        List<ParkSpaceStatusDto> result = new();
        foreach (var item in data.DataList) {
            try {
                var status = item.Unpack<ParkSpaceStatus>();
                var parkSpaceStatusDto= new ParkSpaceStatusDto() {
                    Parkid = Guid.Parse(status.Parkid),
                    Spaceid = status.Spaceid,
                    Status = (ParkStatus)status.Status,
                    DateTime = status.DateTime.ToDateTime()
                };
                result.Add(parkSpaceStatusDto);
            }
            catch (Exception e) {
                _logger.LogInformation("CollectorService: Grpc message could not be deserialized\n{}", e.ToString());
            }
        }
        
        return Task.FromResult(new Empty());
    }
}