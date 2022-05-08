using System.IO.Ports;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Parkla.Business.Abstract;
using Parkla.Core.DTOs;
using Parkla.Web.Options;

namespace Parkla.Web.SerialCom;
public class SerialReceiver : BackgroundService
{
    private readonly object init = new();
    private readonly WebOptions _options;
    private readonly ILogger<SerialReceiver> _logger;
    private readonly ICollectorService _cllectorService;
    private CancellationTokenRegistration? Subscription;
    private readonly JsonSerializerOptions jsonSerializerOptions = new();
    private SerialPort SerialPort { get; set; }
    private StringBuilder StringBuilder { get; set; } = new();
    private int BracketCount = 0;

    public SerialReceiver(
        IOptions<WebOptions> options,
        ILogger<SerialReceiver> logger,
        ICollectorService collectorService
    ) {
        _options = options.Value;
        _logger = logger;
        _cllectorService = collectorService;

        jsonSerializerOptions.AllowTrailingCommas = true;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        jsonSerializerOptions.MaxDepth = 1;
        jsonSerializerOptions.PropertyNameCaseInsensitive = true;
        jsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    }
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Subscription = stoppingToken.Register(() => {
            lock(init) {
                if(Subscription != null ){
                    SerialPort.Dispose();
                    Subscription?.Unregister();
                    Subscription = null;
                }
            }
        });
        
        lock(init) {
            var portName = _options.SerialPortName;
            SerialPort = new SerialPort(portName, 9600);
            SerialPort.DataReceived += DataReceivedHandler;
            SerialPort.Open();
        }

        return Task.CompletedTask;
    }
    
    private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs args) {
        var serialPort = (SerialPort) sender;
        string data;
        try {
            data = serialPort.ReadExisting();
        } catch(Exception e) {
            _logger.LogError(e, "SerialReceiver: Serial port could not be read");
            return;
        }

        var readFrom = 0;
        List<ParkSpaceStatusDto> results = new();

        for(var i=0; i<data.Length; i++) {
            var ch = data[i];

            if(ch == '{' /*&& jsonSerialPort.BracketCount == 0 -> MaxDepth = 1 like jsonserializer*/)
                BracketCount++;
            else if(ch == '}' && BracketCount > 0) {
                BracketCount--;

                if(BracketCount == 0){
                    StringBuilder.Append(data[readFrom..(i + 1)]);
                    readFrom = i+1;

                    var jsonData = StringBuilder.ToString();
                    try {
                        results.Add(JsonSerializer.Deserialize<ParkSpaceStatusDto>(jsonData, jsonSerializerOptions)!);
                    } catch(Exception e) {
                        _logger.LogInformation("DefaultSerialHandler: Invalid json format or unmet constraints received\n{}", e.Message);
                    }
                    StringBuilder.Clear();
                }
            }
            else if(BracketCount == 0) {
                readFrom = i+1;
            }
        }

        if(readFrom < data.Length)
            StringBuilder.Append(data[readFrom..]);
        
        foreach (var dto in results) {
            _logger.LogInformation(
                "SerialReceiver: ParkId='{}' SpaceId='{}' Status='{}' DateTime='{}' is received", 
                dto.ParkId,
                dto.SpaceId,
                dto.Status,
                dto.DateTime
            );
        }

        _cllectorService.CollectParkSpaceStatus(results);
    }
}