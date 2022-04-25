
using System.IO.Ports;
using System.Text.Json;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Exporters;
public class SerialExportManager
{
    private readonly Dictionary<string,SerialPort> _serialPortMap = new();
    private readonly Queue<SerialQueueItem> _exportQueue = new();
    private readonly int _exportDelay = 10000;

    public SerialExportManager()
    {
        ExportAsync();
    }


    public void Export(ParkSpaceStatusDto dto, string portName)
    {
        var isGot = _serialPortMap.TryGetValue(portName,out var serialPort);
        
        if(!isGot) {
            serialPort = new SerialPort(portName,9600);
            _serialPortMap.Add(portName,serialPort);
        }

        _exportQueue.Enqueue(new SerialQueueItem{
            SerialPort = serialPort!,
            Dto = dto
        });
    }

    private async Task ExportAsync() {
        while(_exportQueue.Count > 0) {
            var queueItem = _exportQueue.Dequeue();
            var serialPort = queueItem.SerialPort;
            var dto = queueItem.Dto;

            serialPort.Dispose();
            serialPort.Write(JsonSerializer.Serialize(dto));
        }

        await Task.Delay(_exportDelay);
        ExportAsync();
    }
}