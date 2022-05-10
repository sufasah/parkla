using System.Text.Json.Serialization;
using Parkla.Core.Entities;
using Parkla.Core.Enums;

namespace Parkla.Web.Models;
public class ReceivedSpaceStatusDto
{
    public int? Id { get; set; }
    public int? SpaceId { get; set; }
    public int? RealSpaceId { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SpaceStatus? Status { get; set; }
    public DateTime? DateTime { get; set; }
}