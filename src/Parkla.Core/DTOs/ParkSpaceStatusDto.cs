using Parkla.Core.Enums;

namespace Parkla.Core.DTOs
{
    public class ParkSpaceStatusDto
    {
        public Guid? ParkId { get; set; }
        public int? SpaceId { get; set; }
        public SpaceStatus? Status { get; set; }
        public DateTime? DateTime { get; set; }
    }
}