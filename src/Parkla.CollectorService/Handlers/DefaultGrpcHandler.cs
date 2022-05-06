using Parkla.Protobuf;
using Parkla.CollectorService.Library;
using Parkla.Core.DTOs;
using Parkla.Core.Enums;

namespace Parkla.CollectorService.Handlers;
public class DefualtGrpcHandler : HandlerBase
{    
    public DefualtGrpcHandler() {

    }

    public override IEnumerable<ParkSpaceStatusDto> Handle(ReceiverType receiverType, object param) {
        if(receiverType != ReceiverType.GRPC)
            throw new ArgumentException("DefaultGrpcHandler only handles grpc requests");

        var grpcReceiverParam = (GrpcReceiverParam) param;
        var dataList = grpcReceiverParam.Data.DataList;
        var context = grpcReceiverParam.Context;
        var logger = grpcReceiverParam.Logger;

        List<ParkSpaceStatusDto> result = new();
        foreach (var data in dataList) {
            try {
                var status = data.Unpack<ParkSpaceStatus>();
                var parkSpaceStatusDto= new ParkSpaceStatusDto() {
                    ParkId = Guid.Parse(status.ParkId),
                    SpaceId = status.SpaceId,
                    Status = (SpaceStatus)status.Status,
                    DateTime = status.DateTime.ToDateTime()
                };
                result.Add(parkSpaceStatusDto);
            }
            catch (Exception e) {
                logger.LogInformation("DefaultGrpcHandler: Grpc message could not be deserialized\n{}", e.ToString());
            }
        }

        return result;
    }
}
