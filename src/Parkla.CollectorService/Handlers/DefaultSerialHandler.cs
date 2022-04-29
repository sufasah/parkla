using Parkla.Core.DTOs;
using Parkla.CollectorService.Library;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Parkla.CollectorService.Handlers;
public class DefaultSerialHandler : HandlerBase
{
    private readonly JsonSerializerOptions jsonSerializerOptions = new();
    
    public DefaultSerialHandler()
    {
        jsonSerializerOptions.AllowTrailingCommas = true;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        jsonSerializerOptions.MaxDepth = 1;
        jsonSerializerOptions.PropertyNameCaseInsensitive = true;
        jsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    }

    // THIS HANDLE METHOD WILL BE CALLED WHEN DATARECEIVEDEVENT IS RAISED
    public override IEnumerable<ParkSpaceStatusDto> Handle(ReceiverType receiverType, object param)
    {
        if(receiverType != ReceiverType.SERIAL)
            throw new ArgumentException("ReceiverType is not 'SERIAL'. DefaultSerialHandler only handles serial ports");

        var serialReceiverParam = (SerialReceiverParam) param;
        var serialPort = serialReceiverParam.SerialPort;
        var logger = serialReceiverParam.Logger;

        if(serialReceiverParam.State == null) {
            serialReceiverParam.State = new JsonSerialPort(serialPort);
        }
        
        string data;
        try {
            data = serialPort.ReadExisting();
        } catch(Exception e) {
            logger.LogError(e, "DefaultSerialHandler: Serial port could not be read");
            return Array.Empty<ParkSpaceStatusDto>();
        }

        var jsonSerialPort = (JsonSerialPort)serialReceiverParam.State;
        var readFrom = 0;
        List<ParkSpaceStatusDto> results = new();

        for(var i=0; i<data.Length; i++) {
            var ch = data[i];

            if(ch == '{' /*&& jsonSerialPort.BracketCount == 0 -> MaxDepth = 1 like jsonserializer*/)
                jsonSerialPort.BracketCount++;
            else if(ch == '}' && jsonSerialPort.BracketCount > 0) {
                jsonSerialPort.BracketCount--;

                if(jsonSerialPort.BracketCount == 0){
                    jsonSerialPort.StringBuilder.Append(data[readFrom..(i + 1)]);
                    readFrom = i+1;

                    var jsonData = jsonSerialPort.StringBuilder.ToString();
                    try {
                        results.Add(JsonSerializer.Deserialize<ParkSpaceStatusDto>(jsonData, jsonSerializerOptions)!);
                    } catch(Exception e) {
                        logger.LogInformation("DefaultSerialHandler: Invalid json format or unmet constraints received\n{}", e.Message);
                    }
                    jsonSerialPort.StringBuilder.Clear();
                }
            }
            else if(jsonSerialPort.BracketCount == 0) {
                readFrom = i+1;
            }
        }

        if(readFrom < data.Length)
            jsonSerialPort.StringBuilder.Append(data[readFrom..]);

        return results;
    }
}