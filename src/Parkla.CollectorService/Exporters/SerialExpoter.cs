
using System.Collections.Concurrent;
using System.IO.Ports;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Options;
using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Exporters;
public class SerialExporter
{
    private readonly object _startLock = new();
    private bool Started { get; set; } = false;
    private readonly ConcurrentDictionary<string,SerialPort> _serialPortMap = new();
    private readonly BlockingCollection<SerialQueueItem> _exportQueue = new();
    private readonly ILogger<SerialExporter> _logger;
    private readonly IOptions<CollectorOptions> _options;

    public SerialExporter(
        ILogger<SerialExporter> logger,
        IOptions<CollectorOptions> options
    )
    {
        _logger = logger;
        _options = options;
    }

    public void Start() {
        lock(_startLock) {
            if (Started)
            {
                _logger.LogWarning("SerialExporter.Start: SerialExporter is already started.");
                return;
            }

            foreach (var pipeline in _options.Value.Pipelines)
            {
                var serialExporters = pipeline.SerialExporters;
                if(serialExporters.Length == 0) continue;

                foreach (var serialExporter in serialExporters)
                {
                    try
                    {
                        var serialPort = new SerialPort(serialExporter.PortName, 9600);
                        serialPort.Open();
                        _serialPortMap.TryAdd(serialExporter.PortName ,serialPort);
                    }
                    catch (Exception e)
                    {
                        var contains = _serialPortMap.ContainsKey(serialExporter.PortName);
                        
                        if(!contains)
                            _logger.LogError("SerialReceiver: Serial port with {} port name already in use", serialExporter.PortName);

                        _logger.LogError("SerialExporter: Serial port exporter with {} port name could not be opened. \n{}", serialExporter.PortName, e.Message);
                    }   
                }
            }
            Started = true;
            Task.Run(ExportTask);
            _logger.LogInformation("START: SerialExporter is started");
        }
    }

    public void Enqueue(ParkSpaceStatusDto dto, string portName)
    {
        if(!Started)
            throw new InvalidOperationException("SerialExporter is not started yet.");

        var isGot = _serialPortMap.TryGetValue(portName, out var serialPort);
        
        if(!isGot) {
            _logger.LogWarning("SerialExporter.Enqueue: Serial port exporter could not found  {} port name to enqueue. \nParkId='{}', SpaceId='{}', Status='{}' is not enqueued", portName, dto.Parkid, dto.Spaceid, dto.Spaceid);
            return;
        }

        _exportQueue.Add(new SerialQueueItem {
            SerialPort = serialPort!,
            Dto = dto
        });
    }
    public void Enqueue(IEnumerable<ParkSpaceStatusDto> dtos, string portName)
    {
        foreach(var dto in dtos)
            Enqueue(dto, portName);
    }

    private void ExportTask() {
        while(true) {
            var queueItem = _exportQueue.Take();
            var serialPort = queueItem.SerialPort;
            var dto = queueItem.Dto;

            try {
                serialPort.Write(JsonSerializer.Serialize(dto));
                _logger.LogInformation("SerialExporter [Successful]: ParkId='{}', SpaceId='{}', Status='{}' is exported", dto.Parkid, dto.Spaceid, dto.Status);
            } catch(Exception e) {
                _logger.LogError(e, "SerialExporter [Fail]: ParkId='{}', SpaceId='{}', Status='{}' is not exported", dto.Parkid, dto.Spaceid, dto.Status);
            }
        }
    }
}