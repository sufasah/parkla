using System.Text.Json.Serialization;
using Parkla.Core.Entities;
using Parkla.Core.Enums;

namespace Parkla.Web.Models;
public class RealParkSpaceDto
{
    public int? Id { get; set; }
    public Guid? ParkId { get; set; }
    public int? SpaceId { get; set; }
    public string? Name { get; set; }
    public uint xmin { get; set; }
    public DateTime? StatusUpdateTime { get; set; }
    public SpaceStatus? Status { get; set; }
}