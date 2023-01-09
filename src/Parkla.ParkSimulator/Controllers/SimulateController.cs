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
    public enum ExportType
    {
        SERIAL,
        HTTP,
        GRPC
    };
    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
        MaxDepth = 3,
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private static HttpClient httpClient; // to collector http endpoint
    private static SerialPort serialPort; // to COM2 which is reciever of collector
    private static CollectorClient grpcClient; // to collector

    public readonly static ExportType InitialProtocol = ExportType.HTTP;
    private readonly IConfiguration configuration;
    private static ExportType Protocol = InitialProtocol;

    public SimulateController(IConfiguration configuration)
    {
        this.configuration = configuration;
        if (serialPort == null)
        {
            serialPort = new(GetSerialPort(), 9600);
            serialPort.Open();
        }
        httpClient ??= new();
        grpcClient ??= new(GrpcChannel.ForAddress(GetCollectorEndpoint()));
    }

    private Dictionary<int, string> GetRealSpaces()
    {
        return this.configuration.GetSection("RealSpaces")
            .GetChildren()
            .ToDictionary(x => int.Parse(x.Key), x => x.Value) ?? new();
    }

    private string GetCollectorEndpoint()
    {
        return this.configuration.GetSection("CollectorEndpoint")
            .Value ?? "";
    }

    private string GetSerialPort()
    {
        return this.configuration.GetSection("SerialPort")
            .Value ?? "";
    }

    private string GetParkId()
    {
        return this.configuration.GetSection("ParkId")
            .Value ?? "";
    }

    [HttpGet("/ResetServer")]
    public async Task<IActionResult> ResetServerState()
    {
        foreach (var space in GetRealSpaces())
        {
            await SendAsync(MakeDto(space.Key, SpaceStatus.UNKNOWN));
        }

        return Ok();
    }

    [HttpGet("/SetProtocol")]
    public IActionResult SetProtocol([FromQuery] string protocol)
    {
        if (!System.Enum.IsDefined(typeof(ExportType), protocol))
            return BadRequest($"Given '{protocol}' protocol is not defined");

        Protocol = System.Enum.Parse<ExportType>(protocol);

        return Ok($"Protocol is set as '{Protocol}'");
    }

    [HttpPost("/Space/SetStatus")]
    public async Task<IActionResult> SetSpaceStatus([FromBody] ParkSpaceStatusDto idStatus)
    {
        ParkSpaceStatusDto dto;

        if (idStatus.SpaceId == null)
            return BadRequest("Realspace id not given");

        if (idStatus.Status == null)
            return BadRequest($"Space status is not given");

        dto = MakeDto(idStatus.SpaceId.Value, idStatus.Status.Value);

        await SendAsync(dto);

        return Ok(dto);
    }

    [HttpGet("/Space/Random")]
    public async Task<IActionResult> SetRandom()
    {
        var random = new Random();
        var realSpaces = GetRealSpaces();
        var keys = realSpaces.Keys;
        var spaceId = keys.ElementAt(random.Next(0, keys.Count));
        var status = (SpaceStatus)random.Next(0, 3);

        var dto = MakeDto(spaceId, status);

        await SendAsync(dto);

        return Ok(dto);
    }


    private ParkSpaceStatusDto MakeDto(int realSpaceId, SpaceStatus status)
    {
        return new ParkSpaceStatusDto()
        {
            ParkId = Guid.Parse(GetParkId()),
            SpaceId = realSpaceId,
            Status = status,
            DateTime = DateTime.UtcNow
        };
    }

    private async Task SendAsync(ParkSpaceStatusDto dto)
    {
        switch (Protocol)
        {
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

    private async Task SendHttpAsync(ParkSpaceStatusDto dto)
    {
        var response = await httpClient.PostAsJsonAsync(GetCollectorEndpoint(), dto, jsonSerializerOptions).ConfigureAwait(false);
        if (response.StatusCode != HttpStatusCode.OK)
            throw new Exception("New status could not send");
    }

    private static void SendSerial(ParkSpaceStatusDto dto)
    {
        serialPort.Write(JsonSerializer.Serialize(dto, jsonSerializerOptions));
    }

    private static async Task SendGrpcAsync(ParkSpaceStatusDto dto)
    {
        var data = new Data();
        data.DataList.Add(Any.Pack(
            new ParkSpaceStatus
            {
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