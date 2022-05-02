using System.IO.Ports;
using System.Collections.Concurrent;

namespace Parkla.CollectorService.Generators;
public static class SerialPortPool
{
    // needed fast in searching
    private static readonly ConcurrentDictionary<string, SerialPort> _serialPorts = new();

    public static SerialPort? GetInstance(string portName, ILogger? logger = null) {

        var isGot = _serialPorts.TryGetValue(portName, out var serialPort);

        if(!isGot) {
            try {
                serialPort = new SerialPort(portName, 9600);
                serialPort.Open();
                _serialPorts.TryAdd(portName, serialPort);
            }
            catch (Exception e)
            {
                if(logger != null)
                    logger.LogError(e, "SerialPortPool: Serial port with {} port name could not be opened", portName);
                return null;
            }
        }

        return serialPort;
    }
}