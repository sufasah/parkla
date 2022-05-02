using System.IO.Ports;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Generators;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.OptionsManager;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Receivers;
public class SerialReceiver : ReceiverBase
{
    private readonly ILogger<SerialReceiver> _logger;
    private readonly ParklaOptionsManager _parklaOptionsManager;
    public SerialReceiver(
        ILogger<SerialReceiver> logger,
        ParklaOptionsManager parklaOptionsManager   
    ) : base(logger)
    {
        _logger = logger;
        _parklaOptionsManager = parklaOptionsManager; 
    }
    protected override void DoStart()
    {
        foreach (var pipe in SerialReceiverElem.SerialPipes)
        {
            var receiver = (SerialReceiverElem) pipe.Receiver;
            var serialPort = receiver.SerialPort;
            
            if(serialPort != null)
                serialPort.DataReceived += MakeDataReceived(serialPort, pipe);
        }
    }

    private SerialDataReceivedEventHandler MakeDataReceived(SerialPort serialPort, Pipe serialPipe)
    {
        var param = new SerialReceiverParam
        {
            SerialPort = serialPort,
            State = null,
            Logger = _logger
        };

        return (object sender, SerialDataReceivedEventArgs args) =>
        {
            var handler = serialPipe.Receiver.Handler;
            // _logger.LogInformation("SerialReceiver: Executing handler with name '{}'", handler.GetType().Name);
            try {
                param.SerialDataReceivedEventArgs = args;
                var handlerResults = ResultSafe(handler.HandleAsync(ReceiverType.SERIAL, param)).Result;

                if (handlerResults != null) {
                    ExportResultsAsync(handlerResults, serialPipe.Exporters);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occured while handling the request '{}' handler. The result is not generated so it will not be exported.", handler.GetType().Name);
            }
        };
    }

    private static async Task<IEnumerable<ParkSpaceStatusDto>> ResultSafe(Task<IEnumerable<ParkSpaceStatusDto>> task) {
        // For legacy synchronization context deadlocks configureAwait
        var taskResult = await task.ConfigureAwait(false);
        return taskResult;
    }
}