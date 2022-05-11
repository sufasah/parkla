using System.IO.Ports;

namespace Parkla.SerialTester;

public class Tester {
    public SerialPort SerialIn { get; set; }
    public SerialPort SerialOut { get; set; }
    public Tester()
    {
        SerialIn = new("COM4", 9600);
        SerialIn.DataReceived += DataReceivedHandler;
        
        SerialOut = new("COM1", 9600);

        SerialIn.Open();
        SerialOut.Open();
    }
    public void Test() {
        Console.WriteLine(SerialPort.GetPortNames().Length);
        while(true) {
            var cin = Console.ReadLine();
            if(SerialOut.IsOpen) {
                Console.WriteLine("Written");
                SerialOut.WriteLine(cin!);
            }
        }
    }
    
    public void DataReceivedHandler(object? sender, SerialDataReceivedEventArgs args) {
        var serialPort = (SerialPort) sender!;
        var data = serialPort.ReadExisting();
        Console.WriteLine($"Received data: {data}");
    }

    
}