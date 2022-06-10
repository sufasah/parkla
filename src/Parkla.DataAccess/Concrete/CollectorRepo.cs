using Microsoft.EntityFrameworkCore;
using Parkla.Core.DTOs;
using Parkla.DataAccess.Abstract;
using Parkla.Core.Entities;
using Parkla.Core.Enums;
using Microsoft.Extensions.Logging;

namespace Parkla.DataAccess.Concrete;

public class CollectorRepo<TContext> : ICollectorRepo
    where TContext: DbContext, new()
{
    private readonly ILogger<CollectorRepo<TContext>> _logger;
    public CollectorRepo(ILogger<CollectorRepo<TContext>> logger)
    {
        _logger = logger;
    }

    public async Task<Tuple<ParkSpace, ParkArea, Park>?> CollectParkSpaceStatusAsync(ParkSpaceStatusDto dto)
    {
        using var context = new TContext();
        while(true) {
            try {
                var realSpace = await context.Set<RealParkSpace>()
                    .Include(x => x.Space)
                    .ThenInclude(x => x!.Area)
                    .ThenInclude(x => x!.Park)
                    .ThenInclude(x => x!.User)
                    .Include(x => x.Space)
                    .ThenInclude(x => x!.Pricing)
                    .Where(x => x.Id == dto.SpaceId && x.ParkId == dto.ParkId)
                    .SingleOrDefaultAsync()
                    .ConfigureAwait(false);
                
                if(realSpace == null)
                    throw new KeyNotFoundException();

                if (realSpace.StatusUpdateTime != null && realSpace.StatusUpdateTime > dto.DateTime)
                    throw new TimeoutException();

                var receivedStatus = new ReceivedSpaceStatus
                {
                    RealSpaceId = realSpace.Id,
                    RealSpaceName = realSpace.Name,
                    RealSpace = realSpace,
                    OldRealSpaceStatus = realSpace.Status
                };
                
                var isBounded = realSpace.Space != null;
                ParkSpace? space = null;
                ParkArea? area = null;
                Park? park = null;
                receivedStatus.Space = null;
                receivedStatus.SpaceId = null;
                receivedStatus.SpaceName = null;
                receivedStatus.OldSpaceStatus = null;
                receivedStatus.NewSpaceStatus = null;
                
                if(isBounded) {
                    space = realSpace.Space;
                    area = space!.Area;
                    park = area!.Park;

                    receivedStatus.Space = space;
                    receivedStatus.SpaceId = space.Id;
                    receivedStatus.SpaceName = space.Name;
                    receivedStatus.OldSpaceStatus = space.Status;
                }

                if(isBounded && realSpace.Status != dto.Status) {
                    switch(realSpace.Status) {
                        case SpaceStatus.EMPTY:
                            area!.EmptySpace--;
                            park!.EmptySpace--;
                            break;
                        case SpaceStatus.OCCUPIED:
                            area!.OccupiedSpace--;
                            park!.OccupiedSpace--;
                            break;
                        case SpaceStatus.UNKNOWN:
                            break;
                    }

                    switch(dto.Status) {
                        case SpaceStatus.EMPTY:
                            area!.EmptySpace++;
                            park!.EmptySpace++;
                            break;
                        case SpaceStatus.OCCUPIED:
                            area!.OccupiedSpace++;
                            park!.OccupiedSpace++;
                            break;
                        case SpaceStatus.UNKNOWN:
                            break;
                    }

                    realSpace.Status = space!.Status = dto.Status;

                    receivedStatus.NewRealSpaceStatus = realSpace.Status;
                    receivedStatus.NewSpaceStatus = space!.Status;
                }
                else if(isBounded) {
                    receivedStatus.NewRealSpaceStatus = realSpace.Status;
                    receivedStatus.NewSpaceStatus = space!.Status;
                }

                realSpace.StatusUpdateTime = dto.DateTime;

                receivedStatus.ReceivedTime = DateTime.UtcNow;
                receivedStatus.StatusDataTime = dto.DateTime;

                if(isBounded)
                    space!.StatusUpdateTime = area!.StatusUpdateTime = park!.StatusUpdateTime = dto.DateTime;
                
                context.Add(receivedStatus);

                await context.SaveChangesAsync().ConfigureAwait(false);
                break;
            }
            catch(DbUpdateConcurrencyException) {
                context.ChangeTracker.Clear();
            }
        }

        var result =  context.Set<RealParkSpace>().Local.Single();
        if(result.Space == null)
            return null;

        var resSpace = result.Space;
        var resArea = resSpace.Area!;
        var resPark = resArea.Park!;

        resSpace.Area = null;
        resSpace.ReceivedSpaceStatusses = null!;
        
        resArea.Spaces = null!;
        resArea.Park = null;

        resPark.RealSpaces = null!;
        resPark.Areas = null!;

        return new(resSpace, resArea, resPark);
    }
}