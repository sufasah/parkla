using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class ParkSpaceRepo<TContext> : EntityRepoBase<ParkSpace, TContext>, IParkSpaceRepo
    where TContext : DbContext, new()
{
    public async Task<List<ParkSpace>> GetListAsync(
        int? areaId, 
        bool includeReservations, 
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
        IQueryable<ParkSpace> resultSet = context.Set<ParkSpace>()
            .AsNoTracking()
            .Include(x => x.RealSpace)
            .Include(x => x.Pricing);
        
        if(areaId != null)
            resultSet = resultSet.Where(x => x.AreaId == areaId.Value);
    
        if(includeReservations) {
            resultSet = resultSet.Include(x => x.Reservations!)
                .ThenInclude(x => x.User);
        }

        var result = await resultSet.ToListAsync(cancellationToken).ConfigureAwait(false);

        if(includeReservations) {
            foreach (var space in result)
            {
                foreach (var reservation in space.Reservations!)
                {
                    reservation.User = new User() {
                        Id = reservation.User!.Id,
                        Name = reservation.User!.Name,
                        Surname = reservation.User!.Surname,
                        Username = reservation.User!.Username
                    };
                }

                space.Reservations = space.Reservations.Where(x => x.EndTime > DateTime.UtcNow).OrderBy(x => x.StartTime).ToList();
            }
        }

        return result;
    }
}