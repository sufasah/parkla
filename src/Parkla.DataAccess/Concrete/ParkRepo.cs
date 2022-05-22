using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
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
            .GroupBy(x => x.Id)
            .Select(g => new {
                Park = g.First(x => x.Id == g.Key),
                ReservationCount = g.Sum(
                    x => x.Areas.Sum(
                        y => y.Spaces.Sum(
                            z => z.Reservations!.Where(t => t.EndTime > DateTime.UtcNow).Count())))
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    
        return result.Select(x => {
            x.Park.Areas = null!;
            return new InstantParkReservedSpace(
                x.Park,
                x.ReservationCount
            );
        }).ToList();
    }

    public async Task<List<InstantParkIdReservedSpace>> GetAllParksReservedSpaceCountAsync(CancellationToken cancellationToken = default)
    {
        using var context = new TContext();
        var result = await context.Set<Park>()
            .Include(x => x.Areas!)
            .ThenInclude(x => x.Spaces!)
            .ThenInclude(x => x.Reservations)
            .GroupBy(x => x.Id)
            .Select(g => new {
                ParkId = g.Key,
                ReservationCount = g.Sum(
                    x => x.Areas.Sum(
                        y => y.Spaces.Sum(
                            z => z.Reservations!.Where(t => t.EndTime > DateTime.UtcNow).Count())))
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return result.Select(x => new InstantParkIdReservedSpace(
            x.ParkId!.Value,
            x.ReservationCount
        )).ToList();
    }

    public override async Task<Park> DeleteAsync(Park park, CancellationToken cancellationToken)
    {
        using var context = new TContext();
        try {
            var result = context.Attach(park);
            await result.Reference(x => x.User).LoadAsync(cancellationToken).ConfigureAwait(false);
            result.State = EntityState.Deleted;
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        } catch(DbUpdateConcurrencyException) {
            throw new ParklaConcurrentDeletionException("The park is already deleted by another user");
        }
        return park;
    }
}