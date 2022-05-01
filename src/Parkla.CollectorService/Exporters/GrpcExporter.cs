using System.Text;
using Collector;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.Extensions.Options;
using Parkla.CollectorService.Generators;
using Parkla.CollectorService.Options;
using Parkla.Core.DTOs;
using static Collector.Collector;

namespace Parkla.CollectorService.Exporters;
public class GrpcExporter
{
    private readonly object _startLock = new();
    private bool Started { get; set; } = false;
    private readonly ILogger<GrpcExporter> _logger;
    private readonly IOptions<CollectorOptions> _options;
    private readonly GrpcClientPool _grpcClientPool;
    private readonly Dictionary<string, CollectorClient> _grpcClientMap = new();

    public GrpcExporter(
        ILogger<GrpcExporter> logger,
        IOptions<CollectorOptions> options,
        GrpcClientPool grpcClientPool
    )
    {
        _logger = logger;
        _options = options;
        _grpcClientPool = grpcClientPool;
    }
    
    public void Start() {
        lock(_startLock) {
            if (Started)
            {
                _logger.LogWarning("GrpcExporter.Start: GrpcExporter is already started.");
                return;
            }

            foreach (var pipeline in _options.Value.Pipelines)
            {
                var grpcExporters = pipeline.GrpcExporters;
                if(grpcExporters.Length == 0) continue;

                foreach (var grpcExporter in grpcExporters)
                {
                    var grpcClient = _grpcClientPool.GetInstance(grpcExporter.Address);
                    if(grpcClient == null) {
                        //if address will be available other exporters with same serial port can work but this one's handler won't exist 
                        continue;
                    }

                    _grpcClientMap.TryAdd(grpcExporter.Address, grpcClient);
                }
            }
            Started = true;
            _logger.LogInformation("START: GrpcExporter is started");
        }
    }

    public async Task ExportAsync(ParkSpaceStatusDto dto, string address)
    {
        await ExportAsync(new ParkSpaceStatusDto[]{dto}, address);
    }
    public async Task ExportAsync(IEnumerable<ParkSpaceStatusDto> dtos, string address)
    {
        if(!dtos.Any()) return;

        var isGot = _grpcClientMap.TryGetValue(address, out var grpcClient);
        
        if(!isGot) {
            _logger.LogWarning("GrpcExporter: GrpcChannel could not found with {} address to export", address);
            return;
        }

        var data = new Data();
        string logStr;
        
        foreach (var dto in dtos)
        {
            data.DataList.Add(Any.Pack(
                new ParkSpaceStatus {
                    Parkid = dto.Parkid.ToString(),
                    Spaceid = dto.Spaceid,
                    Status = (int)dto.Status,
                    DateTime = Timestamp.FromDateTime(dto.DateTime.ToUniversalTime())
                }
            ));
        }

        try {
            await grpcClient.ReceiveAsync(data);
            logStr = HttpExporter.LogStrList(dtos, "GrpcExporter", true);
            _logger.LogInformation(logStr);
        }
        catch(Exception e) {
            logStr = HttpExporter.LogStrList(dtos, "GrpcExporter", false);
            _logger.LogError(e,logStr);
        }
    }

}