using System.IO.Ports;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Parkla.Core.DTOs;
using Parkla.Core.Enums;
using Parkla.Protobuf;
using static Parkla.Protobuf.Collector;

namespace Parkla.Web.Controllers;

[ApiController]
[Route("/")]
public class SimulateController : ControllerBase
{
    private enum ExportType {
        SERIAL,
        HTTP,
        GRPC
    };
    private static readonly JsonSerializerOptions jsonSerializerOptions = new(){
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        MaxDepth = 3,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    public static readonly string ParkId = "e23ca377-3f5e-4064-9de1-e23ccae372d9";

    public static readonly List<Tuple<int,string>> RealSpaces = new() {
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
    private static readonly string _collectorEndpoint = "https://localhost:7071"; 
    private static readonly HttpClient httpClient = new(); // to collector http endpoint
    private static readonly SerialPort serialPort = new("COM1", 9600); // to COM2 which is reciever of collector
    private static readonly CollectorClient grpcClient = new(
        GrpcChannel.ForAddress(_collectorEndpoint) // to collector
    );
    
    private ExportType Protocol = ExportType.HTTP;

    [HttpGet("/ResetServer")]
    public async Task<IActionResult> ResetServerState() {
        foreach (var space in RealSpaces)
        {
            await SendAsync(MakeDto(space.Item1,SpaceStatus.UNKNOWN));
        }

        return Ok();
    }

    [HttpGet("/SetProtocol")]
    public IActionResult SetProtocol([FromQuery] string protocol) 
    {
        if(!System.Enum.IsDefined(typeof(ExportType), protocol))
            return BadRequest($"Given '{protocol}' protocol is not defined");

        Protocol = System.Enum.Parse<ExportType>(protocol);

        return Ok($"Protocol is set as '{Protocol}'");
    }

    [HttpPost("/Space/SetStatus")]
    public async Task<IActionResult> SetSpaceStatus([FromBody] ParkSpaceStatusDto idStatus) 
    {
        ParkSpaceStatusDto dto;

        if(idStatus.SpaceId == null)
            return BadRequest("Realspace id not given");
        
        if(idStatus.Status == null)
            return BadRequest($"Space status is not given");

        dto = MakeDto(idStatus.SpaceId.Value, idStatus.Status.Value);

        await SendAsync(dto);

        return Ok(dto);
    }

    [HttpGet("/Space/Random")]
    public async Task<IActionResult> SetRandom() 
    {
        var random = new Random();
        var spaceId = RealSpaces[random.Next(0, RealSpaces.Count)].Item1;
        var status = (SpaceStatus)random.Next(0,3);

        var dto = MakeDto(spaceId, status);

        await SendAsync(dto);

        return Ok(dto);
    }


    private static ParkSpaceStatusDto MakeDto(int realSpaceId, SpaceStatus status) {
        return new ParkSpaceStatusDto() {
            ParkId = Guid.Parse(ParkId),
            SpaceId = realSpaceId,
            Status = status,
            DateTime = DateTime.UtcNow
        };
    }

    private async Task SendAsync(ParkSpaceStatusDto dto) {
        switch(Protocol) {
            case ExportType.SERIAL:
                SendSerial(dto);
                break;
            case ExportType.HTTP:
                await SendHttpAsync(dto).ConfigureAwait(false);
                break;
            case ExportType.GRPC:
                await SendGrpcAsync(dto).ConfigureAwait(false);
                break;
        }
    }

    private static async Task SendHttpAsync(ParkSpaceStatusDto dto) {
        var response = await httpClient.PostAsJsonAsync(_collectorEndpoint, dto, jsonSerializerOptions).ConfigureAwait(false);
        if(response.StatusCode != HttpStatusCode.OK)
            throw new Exception("New status could not send");
    }

    private static void SendSerial(ParkSpaceStatusDto dto) {
        serialPort.Write(JsonSerializer.Serialize(dto, jsonSerializerOptions));
    }

    private static async Task SendGrpcAsync(ParkSpaceStatusDto dto) {
        var data = new Data();
        data.DataList.Add(Any.Pack(
            new ParkSpaceStatus {
                ParkId = dto.ParkId.ToString(),
                SpaceId = dto.SpaceId!.Value,
                Status = (int)dto.Status!,
                DateTime = Timestamp.FromDateTime(dto.DateTime!.Value)
            }
        ));

        data.Group = "test";

        await grpcClient.ReceiveAsync(data).ConfigureAwait(false);
    }
}