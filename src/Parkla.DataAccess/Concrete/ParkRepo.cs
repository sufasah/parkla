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
                ReservedSpaceCount = g.Sum(
                    x => x.Areas.Sum(
                        y => y.Spaces.Sum(
                            z => z.Reservations!.Any(t => t.EndTime > DateTime.UtcNow && t.EndTime < DateTime.UtcNow.AddDays(1).Date) ? 1 : 0)))
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
    
        return result.Select(x => {
            x.Park.Areas = null!;
            return new InstantParkReservedSpace(
                x.Park,
                x.ReservedSpaceCount
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
                ReservedSpaceCount = g.Sum(
                    x => x.Areas.Sum(
                        y => y.Spaces.Sum(
                            z => z.Reservations!.Any(t => t.EndTime > DateTime.UtcNow && t.EndTime < DateTime.UtcNow.AddDays(1).Date) ? 1 : 0)))
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return result.Select(x => new InstantParkIdReservedSpace(
            x.ParkId!.Value,
            x.ReservedSpaceCount
        )).ToList();
    }

    public override async Task<Park> DeleteAsync(Park park, CancellationToken cancellationToken)
    {
        using var context = new TContext();
        while(!cancellationToken.IsCancellationRequested) {
            try {
                var result = context.Attach(park);
                await result.Reference(x => x.User).LoadAsync(cancellationToken).ConfigureAwait(false);
                if(result.State == EntityState.Detached)
                    throw new ParklaConcurrentDeletionException("The park is already deleted by another user");
                result.State = EntityState.Deleted;
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                break;
            } catch(DbUpdateConcurrencyException) {
                context.ChangeTracker.Clear();
            }
        }
        return park;
    }

    public override async Task<Park> UpdateAsync(Park parkParam, CancellationToken cancellationToken) {
        using var context = new TContext();
        var parkClone = parkParam;

        while(!cancellationToken.IsCancellationRequested) {
            try {
                var result = context.Attach(parkClone);
                result.Property(x => x.Name).IsModified = true;
                result.Property(x => x.Location).IsModified = true;
                result.Property(x => x.Latitude).IsModified = true;
                result.Property(x => x.Longitude).IsModified = true;
                result.Property(x => x.Extras).IsModified = true;
                parkClone = (Park)result.CurrentValues.Clone().ToObject();
                var park = result.Entity;

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return park;
            }
            catch(DbUpdateConcurrencyException err) {
                var entry = err.Entries.Single();

                if(entry.Entity is Park entity) {
                    await entry.ReloadAsync(cancellationToken).ConfigureAwait(false);
                    parkClone.xmin = entity.xmin;
                }

                context.ChangeTracker.Clear();
            }
        }

        return parkClone;
    }
}