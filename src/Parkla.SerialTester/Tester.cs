using System.IO.Ports;

namespace Parkla.SerialTester;

public class Tester {
    public SerialPort serialIn { get; set; }
    public SerialPort serialOut { get; set; }
    public Tester()
    {
        serialIn = new("COM4", 9600);
        serialIn.DataReceived += DataReceivedHandler;
        
        serialOut = new("COM1", 9600);

        serialIn.Open();
        serialOut.Open();
    }
    public void Test() {
        Console.WriteLine(SerialPort.GetPortNames().Length);
        while(true) {
            var cin = Console.ReadLine();
            if(serialOut.IsOpen) {
                Console.WriteLine("Written");
                serialOut.WriteLine(cin);
            }
        }
    }
    
    public void DataReceivedHandler(object? sender, SerialDataReceivedEventArgs args) {
        var serialPort = (SerialPort) sender;
        var data = serialPort.ReadExisting();
        Console.WriteLine($"Received data: {data}");
    }

    
}