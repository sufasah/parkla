using System.Text.Json.Serialization;
using Parkla.Core.Entities;
using Parkla.Core.Enums;

namespace Parkla.Web.Models;
public class RealParkSpaceDto
{
    public int? Id { get; set; }
    public int? ParkId { get; set; }
    public int? SpaceId { get; set; }
    public string? Name { get; set; }
    public DateTime? StatusUpdateTime { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SpaceStatus? Status { get; set; }
}