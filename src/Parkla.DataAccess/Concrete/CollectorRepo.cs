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

    public async Task<Tuple<bool,ParkSpace?, Park?>> CollectParkSpaceStatusAsync(ParkSpaceStatusDto dto)
    {
        using var context = new TContext();
        using var transaction = context.Database.BeginTransaction();

        var realSpace = await context.Set<RealParkSpace>()
            .AsTracking()
            .Where(x => x.Id == dto.SpaceId && x.ParkId == dto.ParkId)
            .Include(x => x.Space!)
            .ThenInclude(x => x.Area!)
            .ThenInclude(x => x.Park)
            .FirstOrDefaultAsync();
        
        if(realSpace == null) {
            await transaction.DisposeAsync();
            throw new InvalidDataException();
        }

        if(realSpace.StatusUpdateTime != null && realSpace.StatusUpdateTime > dto.DateTime) {
            await transaction.DisposeAsync();
            return new(false, null, null);
        }

        try {
            var currentStatus = realSpace.Status;
            realSpace.Status = dto.Status;
            realSpace.StatusUpdateTime = dto.DateTime;

            if(realSpace.Space == null) {
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return new(false, null, null);
            }

            var space = realSpace.Space;
            space.Status = dto.Status;
            space.StatusUpdateTime = dto.DateTime;

            var area = space.Area!;
            var park = area.Park!;
            park.StatusUpdateTime = area.StatusUpdateTime = DateTime.UtcNow;
            
            if(currentStatus != dto.Status) {
                switch(currentStatus) {
                    case SpaceStatus.EMPTY:
                        area.EmptySpace -= 1;
                        park.EmptySpace -= 1;
                        break;
                    case SpaceStatus.OCCUPIED:
                        area.OccupiedSpace -= 1;
                        park.OccupiedSpace -= 1;
                        break;
                    case SpaceStatus.UNKNOWN: break;
                }

                switch(dto.Status) {
                    case SpaceStatus.EMPTY:
                        area.EmptySpace += 1;
                        park.EmptySpace += 1;
                        break;
                    case SpaceStatus.OCCUPIED:
                        area.OccupiedSpace += 1;
                        park.OccupiedSpace += 1;
                        break;
                    case SpaceStatus.UNKNOWN: break;
                }
            }

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
            return new(true, space, park);
        } 
        catch(DbUpdateConcurrencyException e) {
            foreach (var entry in e.Entries) {
                var entity = entry.Entity;
                if(entry.Entity is RealParkSpace) {
                    /*
                    var proposedValues = entry.CurrentValues;
                    var databaseValues = entry.GetDatabaseValues();

                    foreach (var property in proposedValues.Properties)
                    {
                        var proposedValue = proposedValues[property];
                        var databaseValue = databaseValues[property];

                        // TODO: decide which value should be written to database
                        // proposedValues[property] = <value to be saved>;
                    }

                    // Refresh original values to bypass next concurrency check
                    entry.OriginalValues.SetValues(databaseValues);
                    */
                }
                else if(entry.Entity is ParkSpace) {

                }
            }
            /*if(entries.whichone.StatusUpdateTime != null and it is > dto.DateTime) {
                await transaction.DisposeAsync();
                return false;
            }*/

            await transaction.RollbackAsync();
            return await CollectParkSpaceStatusAsync(dto);
        }
        catch(Exception e) {
            await transaction.RollbackAsync();
            _logger.LogError(e, "ParkSpaceStatus collector transaction has been failed.");
            return new(false, null, null);
        }

    }
}