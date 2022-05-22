using Parkla.Core.Enums;

namespace Parkla.Core.Entities;

public class ParkSpace : IEntity {
    public ParkSpace()
    {
        ReceivedSpaceStatusses = new HashSet<ReceivedSpaceStatus>();
        Reservations = new HashSet<Reservation>();
    }

    public int? Id { get; set; }
    public int? AreaId { get; set; }
    public virtual ParkArea? Area { get; set; }
    public int? RealSpaceId { get; set; }
    public virtual RealParkSpace? RealSpace { get; set; }
    public string? Name { get; set; }
    public virtual uint xmin { get; set; }
    public DateTime? StatusUpdateTime { get; set; }
    public virtual SpaceStatus? Status { get; set; }
    public int[][]? TemplatePath { get; set; }
    public virtual ICollection<Reservation>? Reservations { get; set; }
    public virtual ICollection<ReceivedSpaceStatus> ReceivedSpaceStatusses { get; set; }

    public override bool Equals(object? obj)
    {
        if(obj == null) return false;
        return Equals((ParkSpace)obj);
    }

    public bool Equals(ParkSpace space) {
        var result = Id == space.Id 
            && AreaId == space.AreaId
            && Area == space.Area
            && RealSpaceId == space.RealSpaceId
            && RealSpace == space.RealSpace
            && Name == space.Name
            && xmin == space.xmin
            && StatusUpdateTime == space.StatusUpdateTime
            && Status == space.Status;
        
        var templatePathEqual = TemplatePath == space.TemplatePath 
            || Enumerable.Range(0,4).All(i => TemplatePath![i].SequenceEqual(space.TemplatePath![i]));
        
        var reservationsEqual = Reservations == space.Reservations || (
            !Reservations!.Except(space.Reservations!).Any()
        ); 
        
        var receivedSpaceStasussesEqual = ReceivedSpaceStatusses == space.ReceivedSpaceStatusses || (
            !ReceivedSpaceStatusses!.Except(space.ReceivedSpaceStatusses!).Any()
        );
        
        return result && templatePathEqual && reservationsEqual && receivedSpaceStasussesEqual;
    }
}