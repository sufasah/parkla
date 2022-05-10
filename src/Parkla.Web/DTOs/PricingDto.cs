using System.Text.Json.Serialization;
using Parkla.Core.Entities;
using Parkla.Core.Enums;

namespace Parkla.Web.Models;
public class PricingDto
{
    public int? Id { get; set; }
    public int? AreaId { get; set; }
    public ParkArea? Area { get; set; }
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public TimeUnit? Unit { get; set; }
    public int? Amount { get; set; }
    public float? Price { get; set; }
}