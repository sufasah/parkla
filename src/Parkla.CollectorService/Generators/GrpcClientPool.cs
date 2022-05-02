using System.Collections.Concurrent;
using Grpc.Net.Client;
using static Parkla.Protobuf.Collector;

namespace Parkla.CollectorService.Generators;
public static class GrpcClientPool
{
    // needed fast in searching
    private static readonly ConcurrentDictionary<string, CollectorClient> _clients = new();
    public static CollectorClient? GetInstance(string address, ILogger? logger = null) {

        var isGot = _clients.TryGetValue(address, out var client);

        if(!isGot) {
            try {
                var channel = GrpcChannel.ForAddress(address);
                client = new CollectorClient(channel);
            }
            catch (Exception e)
            {
                if(logger != null)
                    logger.LogError(e, "GrpcClientPool: GrpcClient with {} address could not be opened", address);
                return null;
            }
        }

        return client;
    }
}