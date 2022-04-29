using System.IO.Ports;
using System.Text;

namespace Parkla.CollectorService
{
    public class JsonSerialPort: IDisposable
    {
        public SerialPort SerialPort { get; set; }
        public StringBuilder StringBuilder { get; set; }
        public int BracketCount { get; set; }

        public JsonSerialPort(SerialPort serialPort)
        {
            SerialPort = serialPort;
            StringBuilder = new StringBuilder();
            BracketCount = 0;
        }

        public void Dispose()
        {
            SerialPort.Dispose();
            GC.SuppressFinalize(this);
        }
    }

}