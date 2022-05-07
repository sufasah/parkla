using Grpc.Core;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.OptionsManager;
using Parkla.Protobuf;

namespace Parkla.CollectorService.Receivers;
public class GrpcReceiver : ReceiverBase
{
    private readonly ILogger<GrpcReceiver> _logger;
    public GrpcReceiver(
        ILogger<GrpcReceiver> logger
    ) : base(logger)
    {
        _logger = logger;
    }

    protected override void DoStart(){}

    public async Task ReceiveAsync(Data data, ServerCallContext context) {
        if (!Started)
            throw new InvalidOperationException("HttpReceiver is not started yet.");
            
        var tasks = new List<Task>();
        
        var grpcPipes = GrpcReceiverElem.GrpcPipes.FindAll(x => {
            var receiver = (GrpcReceiverElem)x.Receiver;
            return receiver.Group == data.Group;
        });

        if(grpcPipes == null) return;

        foreach(var pipe in grpcPipes) {
            var receiver = (GrpcReceiverElem) pipe.Receiver;
            var exporters = pipe.Exporters;
            var handler = receiver.Handler;

            var task = Task.Run(async () => {
                _logger.LogInformation("GrpcReceiver: Executing handler with name '{}'", handler.GetType().Name);
                try {
                    var handlerResults = await handler.HandleAsync(ReceiverType.GRPC, new GrpcReceiverParam {
                        Data = data,
                        Context = context,
                        Logger = _logger
                    }).ConfigureAwait(false);

                    if (handlerResults != null) {
                        ExportResultsAsync(handlerResults, exporters);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occured while handling the request '{}' handler. The result is not generated so it will not be exported.", handler.GetType().Name);
                }
            });
            tasks.Add(task);
        }
        await Task.WhenAll(tasks);
    }
}