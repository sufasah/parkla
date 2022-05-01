using Collector;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Parkla.CollectorService.Library;
public class GrpcReceiverParam : ParamBase
{
    public Data Data { get; set; }
    public ServerCallContext Context { get; set; }
    public ILogger Logger { get; set; }
}