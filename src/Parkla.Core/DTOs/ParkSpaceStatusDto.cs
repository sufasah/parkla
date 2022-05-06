using System.Text.Json.Serialization;
using Parkla.Core.Enums;

namespace Parkla.Core.DTOs
{
    public class ParkSpaceStatusDto
    {
        public Guid ParkId { get; set; }
        public string SpaceId { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public SpaceStatus Status { get; set; }
        public DateTime DateTime { get; set; }
    }
}