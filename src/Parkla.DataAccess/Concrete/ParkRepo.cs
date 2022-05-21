using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.Core.Models;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class ParkRepo<TContext> : EntityRepoBase<Park, TContext>, IParkRepo
    where TContext : DbContext, new()
{
    public async Task<List<InstantParkReservedSpace>> GetAllParksAsync(CancellationToken cancellationToken = default)
    {
        using var context = new TContext();
        var result = await context.Set<Park>()
            .Include(x => x.User)
            .Include(x => x.Areas!)
            .ThenInclude(x => x.Spaces!)
            .ThenInclude(x => x.Reservations)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
        

        return result.Select(x => {
            var innerResult = new InstantParkReservedSpace(
                x,
                x.Areas!.Sum(
                    y => y.Spaces!.Sum(
                        z => z.Reservations!.Count))
            );
            x.Areas = null;
            return innerResult;
        }).ToList();
    }

    public async Task<List<InstantParkIdReservedSpace>> GetAllParksReservedSpaceCountAsync(CancellationToken cancellationToken = default)
    {
        using var context = new TContext();
        var reservations = await context.Set<Reservation>()
            .Include(x => x.Space!)
            .ThenInclude(x => x.Area!)
            .ThenInclude(x => x.Park)
            .Where(x => x.StartTime <= DateTime.UtcNow)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        await context.Set<Park>().ToListAsync(cancellationToken).ConfigureAwait(false);
        
        
        var result = context.ChangeTracker.Entries<Park>().Select(x => new {
            ParkId = x.Entity.Id,
            Count = x.Entity.Areas == null ? 0 : x.Entity.Areas.Sum(
                y => y.Spaces!.Sum(
                    z => z.Reservations!.Count))
        });

        return result.Select(x => new InstantParkIdReservedSpace(x.ParkId!.Value, x.Count)).ToList();
    }
}