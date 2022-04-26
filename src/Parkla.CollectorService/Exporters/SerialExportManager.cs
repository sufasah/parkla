
using System.IO.Ports;
using System.Text.Json;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Exporters;
public class SerialExportManager
{
    private readonly Dictionary<string,SerialPort> _serialPortMap = new();
    private readonly Queue<SerialQueueItem> _exportQueue = new();
    private readonly int _exportDelay = 10000;
    private readonly ILogger<SerialExportManager> _logger;

    public SerialExportManager(
        ILogger<SerialExportManager> logger
    )
    {
        _logger = logger;
        ExportAsync();
    }


    public void Enqueue(ParkSpaceStatusDto dto, string portName)
    {
        var isGot = _serialPortMap.TryGetValue(portName, out var serialPort);
        
        if(!isGot) {
            serialPort = new SerialPort(portName,9600);
            _serialPortMap.Add(portName,serialPort);
        }

        _exportQueue.Enqueue(new SerialQueueItem{
            SerialPort = serialPort!,
            Dto = dto
        });
    }
    public void Enqueue(IEnumerable<ParkSpaceStatusDto> dtos, string portName)
    {
        foreach(var dto in dtos)
            Enqueue(dto, portName);
    }

    private async Task ExportAsync() {
        while(_exportQueue.Count > 0) {
            var queueItem = _exportQueue.Dequeue();
            var serialPort = queueItem.SerialPort;
            var dto = queueItem.Dto;

            serialPort.Write(JsonSerializer.Serialize(dto));
            _logger.LogInformation("SerialExporter [Successful]: ParkId='{}', SpaceId='{}', Status='{}' is exported", dto.Parkid, dto.Spaceid, dto.Status);
        }

        await Task.Delay(_exportDelay);
        ExportAsync();
    }
}