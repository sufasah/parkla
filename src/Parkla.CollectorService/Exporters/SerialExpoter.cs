using System.Collections.Concurrent;
using System.IO.Ports;
using System.Text.Json;
using System.Text.Json.Serialization;
using Parkla.CollectorService.OptionsManager;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Exporters;
public class SerialExporter : ExporterBase
{
    private readonly BlockingCollection<SerialQueueItem> _exportQueue = new();
    private readonly ILogger<SerialExporter> _logger;
    private readonly JsonSerializerOptions jsonSerializerOptions = new();

    public SerialExporter(
        ILogger<SerialExporter> logger
    ) : base(logger)
    {
        _logger = logger;

        jsonSerializerOptions.AllowTrailingCommas = true;
        jsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault;
        jsonSerializerOptions.MaxDepth = 3;
        jsonSerializerOptions.PropertyNameCaseInsensitive = true;
        jsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    }

    protected override void DoStart() {
        Task.Run(ExportTask);
    }

    public override Task ExportAsync(ParkSpaceStatusDto dto, ExporterElemBase exporterElemBase)
    {
        var exporterElem = (SerialExporterElem) exporterElemBase;
        Enqueue(dto, exporterElem.SerialPort);
        return Task.CompletedTask;
    }

    public override Task ExportAsync(IEnumerable<ParkSpaceStatusDto> dtos, ExporterElemBase exporterElemBase)
    {
        var exporterElem = (SerialExporterElem) exporterElemBase;
        foreach (var dto in dtos)
        {
            Enqueue(dto, exporterElem.SerialPort);
        }
        return Task.CompletedTask;
    }

    public void Enqueue(ParkSpaceStatusDto dto, SerialPort? serialPort)
    {
        if(!Started)
            throw new InvalidOperationException("SerialExporter is not started yet.");

        if(serialPort == null) {
            _logger.LogWarning("SerialExporter.Enqueue: SerialPortExporter could not found the SerialPort to enqueue. \nParkId='{}', SpaceId='{}', Status='{}' is not enqueued", dto.Parkid, dto.Spaceid, dto.Spaceid);
            return;
        }

        _exportQueue.Add(new SerialQueueItem {
            SerialPort = serialPort,
            Dto = dto
        });
    }
    
    private void ExportTask() {
        while(true) {
            var queueItem = _exportQueue.Take();
            var serialPort = queueItem.SerialPort;
            var dto = queueItem.Dto;

            try {
                serialPort.Write(JsonSerializer.Serialize(dto, jsonSerializerOptions));
                _logger.LogInformation("SerialExporter [Successful]: ParkId='{}', SpaceId='{}', Status='{}' is exported", dto.Parkid, dto.Spaceid, dto.Status);
            } catch(Exception e) {
                _logger.LogError(e, "SerialExporter [Fail]: ParkId='{}', SpaceId='{}', Status='{}' is not exported", dto.Parkid, dto.Spaceid, dto.Status);
            }
        }
    }

   
}