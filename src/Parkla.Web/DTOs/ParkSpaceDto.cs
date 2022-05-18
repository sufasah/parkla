using Parkla.Core.Enums;

namespace Parkla.Web.Models;
public class ParkSpaceDto
{
    public int? Id { get; set; }
    public int? AreaId { get; set; }
    public int? RealSpaceId { get; set; }
    public string? Name { get; set; }
    public DateTime? StatusUpdateTime { get; set; }
    public SpaceStatus? Status { get; set; }
    public int[][]? SpacePath { get; set; }
}