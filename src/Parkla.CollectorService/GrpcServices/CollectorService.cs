using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Parkla.CollectorService.Receivers;

namespace Collector;
public class CollectorService : Collector.CollectorBase
{
    private readonly GrpcReceiver _grpcReceiver;

    public CollectorService(GrpcReceiver grpcReceiver) {
        _grpcReceiver = grpcReceiver;
    }
    public override async Task<Empty> Receive(Data data, ServerCallContext context)
    {
        await _grpcReceiver.ReceiveAsync(data, context);
        return new Empty();
    }
}