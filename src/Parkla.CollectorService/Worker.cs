using System.IO.Ports;
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IOptionsMonitor<CollectorOptions> _collectorOptionsMonitor;
    private readonly List<JsonSerialPort> _rPorts = new();
    private readonly List<JsonSerialPort> _ePorts = new();
    private CancellationToken StoppingToken { get; set; }
    private readonly object PortsLock = new();
    private CancellationTokenRegistration Registration;
    private readonly List<IDisposable> _disposables = new();

    public Worker(
        ILogger<Worker> logger, 
        IOptionsMonitor<CollectorOptions> collectorOptionsMonitor)
    {
        _logger = logger;
        _collectorOptionsMonitor = collectorOptionsMonitor;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        StoppingToken = stoppingToken;

        Registration = StoppingToken.Register(() => {
            lock(PortsLock) {
                ClearSerialPorts();
            }
        });

        ReadOptions();

        var listener = _collectorOptionsMonitor.OnChange((options) => {
            lock(PortsLock) {
                ClearSerialPorts();
                ReadOptions();
            }
        });

        _disposables.Add(listener);
    }

    public void ReadOptions() {
        /*var collectorOptions = _collectorOptionsMonitor.CurrentValue;
        
        foreach(var port in collectorOptions.Export.Serial.Ports) {
            lock(PortsLock) {
                if(StoppingToken.IsCancellationRequested) return;
                var jsonSerialPort = new JsonSerialPort(new(port,9600)); 
                _ePorts.Add(jsonSerialPort);
            }
        }

        foreach(var port in collectorOptions.Receive.Serial.Ports) {
            lock(PortsLock) {
                if(StoppingToken.IsCancellationRequested) return;
                var jsonSerialPort = new JsonSerialPort(new(port,9600)); 
                _rPorts.Add(jsonSerialPort);
                jsonSerialPort.SerialPort.DataReceived += DataReceivedHandler;
            }
        }*/
    }

    public void DataReceivedHandler (object sender, SerialDataReceivedEventArgs args) {
        lock(PortsLock) {
            var serialPort = (SerialPort) sender;
            var data = serialPort.ReadExisting();
            var jsonSerialPort = FindJsonSerialPort(serialPort)!;
            var readFrom = 0;

            for(var i=0; i<data.Length; i++) {
                var ch = data[i];

                if(ch == '{')
                    jsonSerialPort.BracketCount++;
                    
                if(ch == '}') {
                    jsonSerialPort.BracketCount--;
                    if(jsonSerialPort.BracketCount == 0){
                        jsonSerialPort.StringBuilder.Append(data[readFrom..(i + 1)]);
                        readFrom = i+1;
                        OnJsonFound(jsonSerialPort);
                    }
                }
            }

            if(readFrom < data.Length)
                jsonSerialPort.StringBuilder.Append(data[readFrom..]);
        }
    }

    public void OnJsonFound(JsonSerialPort jsonSerialPort) {
        var jsonData = jsonSerialPort.StringBuilder.ToString();
        jsonSerialPort.StringBuilder.Clear();
        _ePorts.ForEach(ePort => ePort.SerialPort.Write(jsonData));
    }

    public JsonSerialPort? FindJsonSerialPort(SerialPort serial) {
        return _rPorts.Find(x => x.SerialPort == serial);
    }

    public void ClearSerialPorts() {
        _rPorts.ForEach(rPort => rPort.SerialPort.Close());
        _ePorts.ForEach(ePort => ePort.SerialPort.Close());
        _rPorts.Clear();
        _ePorts.Clear();
    }

    void Dispose() {
        Registration.Unregister();
        base.Dispose();
    }
}
