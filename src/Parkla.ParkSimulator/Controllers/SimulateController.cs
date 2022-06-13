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
    public enum ExportType {
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
        new(80, "rs176"),
        new(81, "rs177"),
        new(82, "rs178"),
        new(83, "rs179"),
        new(84, "rs180"),
        new(85, "rs181"),
        new(86, "rs182"),
        new(87, "rs183"),
        new(88, "rs184"),
        new(89, "rs185"),
        new(90, "rs186"),
        new(91, "rs187"),
        new(92, "rs188"),
    };
    private static readonly string _collectorEndpoint = "https://localhost:7071"; 
    private static readonly HttpClient httpClient = new(); // to collector http endpoint
    private static readonly SerialPort serialPort = new("COM3", 9600); // to COM2 which is reciever of collector
    private static readonly CollectorClient grpcClient = new(
        GrpcChannel.ForAddress(_collectorEndpoint) // to collector
    );
    
    public readonly static ExportType InitialProtocol = ExportType.HTTP;

    private static ExportType Protocol = InitialProtocol;

    static SimulateController() {
        serialPort.Open();
    }

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