using System.Text.Json.Serialization;
using Parkla.Core.Enums;

namespace Parkla.Core.DTOs
{
    public class ParkSpaceStatusDto
    {
        public Guid Parkid { get; set; }
        public string Spaceid { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ParkStatus Status { get; set; }
    }
}