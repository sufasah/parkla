using System.IO.Ports;
using System.Collections.Concurrent;

namespace Parkla.CollectorService.Generators;
public class SerialPortPool
{
    // needed fast in searching
    private readonly ConcurrentDictionary<string, SerialPort> _serialPorts = new();
    private readonly ILogger<SerialPortPool> _logger;

    public SerialPortPool(
        ILogger<SerialPortPool> logger
    )
    {
        _logger = logger;
    }
    public SerialPort? GetInstance(string portName) {

        var isGot = _serialPorts.TryGetValue(portName, out var serialPort);

        if(!isGot) {
            try {
                serialPort = new SerialPort(portName, 9600);
                serialPort.Open();
                _serialPorts.TryAdd(portName, serialPort);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "SerialReceiver: Serial port receiver with {} port name could not be opened. \n", portName);
                return null;
            }
        }

        return serialPort;
    }
}