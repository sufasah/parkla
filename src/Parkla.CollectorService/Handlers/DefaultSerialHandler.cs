using Parkla.Core.DTOs;
using Parkla.CollectorService.Library;
using System.IO.Ports;
using System.Text.Json;

namespace Parkla.CollectorService.Handlers;
public class DefaultSerialHandler : HandlerBase
{
    // THIS HANDLE METHOD WILL BE CALLED WHEN DATARECEIVEDEVENT IS RAISED
    public override IEnumerable<ParkSpaceStatusDto> Handle(ReceiverType receiverType, object param)
    {
        if(receiverType != ReceiverType.SERIAL)
            throw new ArgumentException("DefaultSerialHandler only handles serial ports");

        var serialReceiverParam = (SerialReceiverParam) param;
        var serialPort = serialReceiverParam.SerialPort;

        if(serialReceiverParam.State == null) {
            serialReceiverParam.State = new JsonSerialPort(serialPort);
        }

        var data = serialPort.ReadExisting();
        var jsonSerialPort = (JsonSerialPort)serialReceiverParam.State;
        var readFrom = 0;
        List<ParkSpaceStatusDto> results = new();

        for(var i=0; i<data.Length; i++) {
            var ch = data[i];

            if(ch == '{')
                jsonSerialPort.BracketCount++;
                
            if(ch == '}') {
                jsonSerialPort.BracketCount--;
                if(jsonSerialPort.BracketCount == 0){
                    jsonSerialPort.StringBuilder.Append(data[readFrom..(i + 1)]);
                    readFrom = i+1;

                    var jsonData = jsonSerialPort.StringBuilder.ToString();
                    results.Add(JsonSerializer.Deserialize<ParkSpaceStatusDto>(jsonData)!);
                    jsonSerialPort.StringBuilder.Clear();
                }
            }
        }

        if(readFrom < data.Length)
            jsonSerialPort.StringBuilder.Append(data[readFrom..]);

        return results;
    }
}