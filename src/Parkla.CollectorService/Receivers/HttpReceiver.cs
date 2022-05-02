using System.Collections.ObjectModel;
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.OptionsManager;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.Receivers;
public class HttpReceiver : ReceiverBase
{
    private readonly object _startLock = new();
    private readonly ILogger<HttpReceiver> _logger;
    private readonly ParklaOptionsManager _parklaOptionsManager;

    private bool Started { get; set; } = false;

    public HttpReceiver(
        ILogger<HttpReceiver> logger,
        ParklaOptionsManager parklaOptionsManager
    ) : base(logger)
    {
        _logger = logger;
        _parklaOptionsManager = parklaOptionsManager;
    }

    protected override void DoStart(){}

    public async Task ReceiveAsync(HttpContext context, Pipe[] httpPipes)
    {
        if (!Started)
            throw new InvalidOperationException("HttpReceiver is not started yet.");
        
        var tasks = new List<Task>();
        foreach (var pipe in httpPipes)
        {
            var receiver = (HttpReceiverElem) pipe.Receiver;
            var handler = receiver.Handler;
            var exporters = pipe.Exporters;

            var task = Task.Run(async () =>
            {
                _logger.LogInformation("HttpReceiver: Executing handler with name '{}' for path '{}'", handler.GetType().Name, receiver.Endpoint);
                try
                {
                    var handlerResults = await handler.HandleAsync(ReceiverType.HTTP, new HttpReceiverParam {
                            HttpContext = context,
                            Logger = _logger
                        }).ConfigureAwait(false);

                    if (handlerResults != null)
                    {
                        ExportResultsAsync(handlerResults, exporters);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An error occured while handling the request with '{}' handler for '{}' path. The result is not generated so not exported.", handler.GetType().Name, receiver.Endpoint);
                }
            });

            tasks.Add(task);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
    }
}