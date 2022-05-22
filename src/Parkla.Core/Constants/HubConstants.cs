namespace Parkla.Core.Constants;
public static class HubConstants
{
    public static readonly string EventAllParksStream = "AllParksStream";

    public static readonly string EventParkChanges = "ParkAddUpdateDelete";
    public static readonly string EventParkChangesGroup = "ParkAddUpdateDeleteGroup";

    public static readonly string EventParkSpaceChanges = "ParkSpaceAddUpdateDelete";
    public static string EventParkSpaceChangesGroup(int areaId) => $"ParkSpaceAddUpdateDeleteGroup/{areaId}";

    public static readonly string EventReservationChanges = "ReservationAddUpdateDelete";
    public static string EventReservationChangesGroup(int areaId) => $"ReservationAddUpdateDeleteGroup/{areaId}";
    
    public static readonly string EventParkAreaChanges = "ParkAreaAddUpdateDelete";
    public static string EventParkAreaChangesGroup(Guid parkId) => $"ParkAreaAddUpdateDeleteGroup/{parkId}";
}