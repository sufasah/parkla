using System.Text.Json.Serialization;
using Parkla.CollectorService.Enums;

namespace Parkla.CollectorService.DTOs
{
    public class ParkSpaceStatusDto
    {
        public Guid Parkid { get; set; }
        public string Spaceid { get; set; }
        
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public ParkStatus Status { get; set; }
    }
}