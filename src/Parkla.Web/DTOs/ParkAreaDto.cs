namespace Parkla.Web.Models;
public class ParkAreaDto
{
    public int? Id { get; set; }
    public int? ParkId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? TemplateImage { get; set; }
    public bool? ReservationsEnabled { get; set; }
}