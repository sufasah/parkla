using System.Collections.Concurrent;
using Grpc.Net.Client;
using static Parkla.Protobuf.Collector;

namespace Parkla.CollectorService.Generators;
public class GrpcClientPool
{
    // needed fast in searching
    private readonly ConcurrentDictionary<string, CollectorClient> _clients = new();
    private readonly ILogger<GrpcClientPool> _logger;

    public GrpcClientPool(
        ILogger<GrpcClientPool> logger
    )
    {
        _logger = logger;
    }
    public CollectorClient? GetInstance(string address) {

        var isGot = _clients.TryGetValue(address, out var client);

        if(!isGot) {
            try {
                var channel = GrpcChannel.ForAddress(address);
                client = new CollectorClient(channel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GrpcClientPool: GrpcClient with {} address could not be opened", address);
                return null;
            }
        }

        return client;
    }
}