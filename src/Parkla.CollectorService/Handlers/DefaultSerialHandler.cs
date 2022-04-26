using Parkla.Core.DTOs;
using Parkla.CollectorService.Library;

namespace Parkla.CollectorService.Handlers;
public class DefaultSerialHandler : HandlerBase
{
    // THIS HANDLE METHOD WILL BE CALLED WHEN DATARECEIVEDEVENT IS RAISED
    public override ParkSpaceStatusDto Handle(ReceiverType receiverType, object param)
    {
        throw new NotImplementedException();
    }

    /*public void Receive (object sender, SerialDataReceivedEventArgs args) {
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

    public void OnJsonFound(JsonSerialPort jsonSerialPort) {
        var jsonData = jsonSerialPort.StringBuilder.ToString();
        jsonSerialPort.StringBuilder.Clear();
    }*/
}