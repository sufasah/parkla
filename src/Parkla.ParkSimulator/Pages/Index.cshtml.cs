using System.IO.Ports;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Parkla.Core.DTOs;
using Parkla.Core.Enums;
using Parkla.Protobuf;
using static Parkla.Protobuf.Collector;

namespace Parkla.ParkSimulator.Pages;

public class IndexModel : PageModel
{

    private readonly JsonSerializerOptions jsonSerializerOptions = new(){
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        MaxDepth = 3,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private readonly ILogger<IndexModel> _logger;

    public string ParkId = "88ffd7d1-d194-4781-bf02-ae3ecd3327d7";

    public List<Tuple<int,string>> RealSpaces = new() {
        new(12, "rs12"),
        new(13, "rs13"),
        new(14, "rs14"),
        new(15, "rs15"),
        new(16, "rs16"),
        new(17, "rs17"),
        new(18, "rs18"),
        new(19, "rs19"),
        new(20, "rs20"),
        new(21, "rs21"),
        new(23, "rs23"),
        new(24, "rs24"),
        new(25, "rs25"),
    };

    private readonly HttpClient httpClient = new(); // to collector http endpoint
    private readonly SerialPort serialPort = new("COM1", 9600); // TO com2
    private readonly CollectorClient grpcClient = new(
        GrpcChannel.ForAddress("https://localhost:7071") // collector
    );

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {

    }

    public ParkSpaceStatusDto Dto(int realSpaceId, SpaceStatus? istatus = null, DateTime? time = null) {
        var random = new Random(12);
        var datetime = DateTime.UtcNow.Subtract(new TimeSpan(0,0,
            random.Next(0,3600)
        ));
        var status = (SpaceStatus)random.Next(0,3);

        return new ParkSpaceStatusDto() {
            ParkId = Guid.Parse(ParkId),
            SpaceId = realSpaceId,
            Status = istatus ?? status,
            DateTime = time ?? datetime
        };
    }

    public async Task SendHttpAsync(ParkSpaceStatusDto dto) {
        await httpClient.PostAsJsonAsync("https://localhost:7071", dto, jsonSerializerOptions);
    }

    public void SendSerial(ParkSpaceStatusDto dto) {
        serialPort.Write(JsonSerializer.Serialize(dto, jsonSerializerOptions));
    }

    public async Task SendGrpcAsync(ParkSpaceStatusDto dto) {
        var data = new Data();
        data.DataList.Add(Any.Pack(
            new ParkSpaceStatus {
                ParkId = dto.ParkId.ToString(),
                SpaceId = dto.SpaceId!.Value,
                Status = (int)dto.Status!,
                DateTime = Timestamp.FromDateTime(dto.DateTime!.Value)
            }
        ));

        await grpcClient.ReceiveAsync(data);
    }

}
