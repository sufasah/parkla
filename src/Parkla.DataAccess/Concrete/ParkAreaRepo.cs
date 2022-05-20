using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Parkla.Core.Entities;
using Parkla.Core.Enums;
using Parkla.Core.Exceptions;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class ParkAreaRepo<TContext> : EntityRepoBase<ParkArea, TContext>, IParkAreaRepo 
    where TContext: DbContext, new()
{
    private static readonly ParklaException _parkNotFound = new("Park of the adding area so also this area does not exist in database.", HttpStatusCode.BadRequest);

    private static async Task<Tuple<float?,float?,float?>> FindNewParkMinAvgMaxAsync(
        TContext context,
        ParkArea area,
        CancellationToken cancellationToken = default
    ) {
        var mmaResult = await context.Set<ParkArea>()
            .AsNoTracking()
            .Where(x => x.ParkId == area.ParkId && x.Id != area.Id)
            .GroupBy(_ => 1)
            .Select(x => new {
                Count = x.Count(),
                Min = x.Min(y => y.MinPrice),
                Avarage = x.Average(y => y.AvaragePrice),
                Max = x.Max(y => y.MaxPrice)
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
            
        var newMin = mmaResult[0].Min;
        var newAvarage = mmaResult[0].Avarage;
        var newMax = mmaResult[0].Max;
        var count = (float) mmaResult[0].Count;
        
        if(area.AvaragePrice != null)
        newAvarage = count > 0 && newAvarage != null 
            ? newAvarage * (count / (count+1)) + area.AvaragePrice / (count + 1)
            : area.AvaragePrice;
                
        if(area.MinPrice != null)
        newMin = newMin != null
            ? Math.Min((float)newMin, (float)area.MinPrice)
            : area.MinPrice;

        if(area.MaxPrice != null)
        newMax = newMax != null
            ? Math.Max((float)newMax, (float)area.MaxPrice)
            : area.MaxPrice;
        
        return new(newMin, newAvarage, newMax);
    }

    private static void SetXMin(EntityEntry entry, PropertyValues dbValues) 
    {
        var pxmin = entry.Property("xmin")!;
        pxmin.CurrentValue =  dbValues.GetValue<uint>("xmin");
        pxmin.OriginalValue =  dbValues.GetValue<uint>("xmin");
    }

    private static async Task<bool> ReloadParkCheckMinAvgMax(EntityEntry parkEntry, CancellationToken cancellationToken = default) {
        var park = (Park)parkEntry.Entity;
        var originals = (Park)parkEntry.OriginalValues.Clone().ToObject();
        await parkEntry.ReloadAsync(cancellationToken).ConfigureAwait(false);

        if(parkEntry.State == EntityState.Detached)
            throw _parkNotFound;
        
        if(
            originals.MinPrice != park.MinPrice || 
            originals.AvaragePrice != park.AvaragePrice || 
            originals.MaxPrice != park.MaxPrice
        ) {
            return true;
        }
        return false;
    }
    
    private static async Task<Park> InitParkAndAreaMinAvgMax(TContext context, ParkArea area, CancellationToken cancellationToken = default) {
        var park = await context.FindAsync<Park>(new object?[]{area.ParkId}, cancellationToken: cancellationToken).ConfigureAwait(false);

        if(park == null)
            throw _parkNotFound;
        
        (area.MinPrice, area.AvaragePrice, area.MaxPrice) = Pricing.FindMinAvgMax(area.Pricings);
        (park.MinPrice, park.AvaragePrice, park.MaxPrice) = await FindNewParkMinAvgMaxAsync(context, area, cancellationToken).ConfigureAwait(false);
        return park;
    }

    public new async Task<Tuple<ParkArea, Park?>> AddAsync(ParkArea area, CancellationToken cancellationToken = default) {
        using var context = new TContext();
        var result = context.Add(area);        
        var park = await InitParkAndAreaMinAvgMax(context, area, cancellationToken).ConfigureAwait(false);

        var cancelled = await RetryOnConcurrencyErrorAsync(async () => 
        {
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return false;
        }, 
        async (err) => 
        {
            var entry = err.Entries.Single();

            if(entry.Entity is Park park) {
                var changed = await ReloadParkCheckMinAvgMax(entry, cancellationToken).ConfigureAwait(false);
                if(changed) 
                    (park.MinPrice, park.AvaragePrice, park.MaxPrice) = await FindNewParkMinAvgMaxAsync(context, area, cancellationToken).ConfigureAwait(false);
            }
            
            return true;
        }, cancellationToken).ConfigureAwait(false);

        if(cancelled) return new(area, null);
        return new(area, park);
    }

    private static async Task RetryDeletedPricingsAsync(
        TContext context,
        ParkArea area,
        List<Pricing> deletingPricings,
        List<User> updatingUsersWallet,
        CancellationToken cancellationToken = default
    ) {
        // clearing the state. This state will be filled while retrying.
        // Maybe pricings here deleted from database before this turn so for this turn the search and user updates are done again   
        foreach (var item in deletingPricings)
            context.Entry(item).State = EntityState.Detached;
        foreach (var item in updatingUsersWallet)
            context.Entry(item).State = EntityState.Detached;
        deletingPricings.Clear();
        updatingUsersWallet.Clear();
        
        var newDeletingPricings = await context.Set<Pricing>()
            .Where(x => x.AreaId == area.Id && !area.Pricings!.Contains(x))
            .Include(x => x.Reservations!)
            .ThenInclude(x => x.User)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);         
        context.RemoveRange(newDeletingPricings);

        var resUsers = newDeletingPricings.SelectMany(x => x.Reservations!.Select(y => new {
            Reservation = y,
            User = y.User!
        }));

        foreach (var resUser in resUsers) {
            var reservation = resUser.Reservation;
            var timeIntervalAsHour = reservation.EndTime!.Value.Subtract(reservation.StartTime!.Value).TotalHours;
            reservation.User!.Wallet += Pricing.GetPricePerHour(reservation.Pricing!) * (float)timeIntervalAsHour;
        }

        var newUpdatingUsersWallet = resUsers.Select(x => x.User).ToList();

        deletingPricings.AddRange(newDeletingPricings);
        updatingUsersWallet.AddRange(newUpdatingUsersWallet);
    }

    public new async Task<Tuple<ParkArea, Park?>> UpdateAsync(
        ParkArea area,
        Expression<Func<ParkArea, object?>>[] updateProps,
        bool updateOtherProps = true,
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
        var result = context.Update(area);

        result.State = updateOtherProps ? EntityState.Modified : EntityState.Unchanged;
        foreach (var prop in updateProps)
            result.Property(prop).IsModified = !updateOtherProps;
            
        var park = await InitParkAndAreaMinAvgMax(context, area, cancellationToken).ConfigureAwait(false);
        
        var deletingPricings = new List<Pricing>();
        var updatingUsersWallet = new List<User>();

        var cancelled = await RetryOnConcurrencyErrorAsync(async () => 
        {
            await RetryDeletedPricingsAsync(context, area, deletingPricings, updatingUsersWallet, cancellationToken).ConfigureAwait(false);
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return false;
        }, 
        async (err) => 
        {
            var entry = err.Entries.Single();

            if(entry.Entity is Pricing pri) 
            {
                // it must be deleted before update or delete(also deletion is kinda update) so record not found. Because there is no concurrency token.
                // insert it as a new record if it is pricing of updating area else no need to track it 
                if(area.Pricings!.Contains(pri)) {
                    pri.Id = null;
                    entry.State = EntityState.Added;
                }
                else entry.State = EntityState.Detached;
            }
            else if(entry.Entity is Park) 
            {
                var changed = await ReloadParkCheckMinAvgMax(entry, cancellationToken).ConfigureAwait(false);
                if(changed) 
                    (park.MinPrice, park.AvaragePrice, park.MaxPrice) = await FindNewParkMinAvgMaxAsync(context, area, cancellationToken).ConfigureAwait(false);
            }
            else if(entry.Entity is User user) {
                // wallet of user could not incremented it must be updated or deleted 
                // if it is deleted no need to update wallet so detach else do it for new version
                await entry.ReloadAsync(cancellationToken).ConfigureAwait(false);
            }
            else if(entry.Entity is ParkArea area) {
                // update or delete opreation but it is update because this function called on area update request
                // there is concurrency token for area so either version mismatch or record not found
                var dbval = await entry.GetDatabaseValuesAsync(cancellationToken).ConfigureAwait(false);
                if(dbval == null)
                    throw new ParklaConcurrentDeletionException("The park area trying to update is already updated by another user");
                SetXMin(entry, dbval);
            }
            return true;
        }, cancellationToken).ConfigureAwait(false);

        if(cancelled) return new(area, null);
        return new(area, park);
    }


    public override async Task<Park?> DeleteAsync(ParkArea area, CancellationToken cancellationToken = default)
    {
        using var context = new TContext();

        var park = await InitParkAndAreaMinAvgMax(context, new ParkArea(){
            Id = area.Id,
            ParkId = area.ParkId,
            Pricings = Array.Empty<Pricing>()
        }, cancellationToken).ConfigureAwait(false);

        var cancelled = await RetryOnConcurrencyErrorAsync(async () => 
        {
            var result = await context.Set<ParkArea>().Where(x => x.Id == area.Id)
                .Include(x => x.Pricings!)
                .ThenInclude(x => x.Reservations!)
                .ThenInclude(x => x.User)
                .Include(x => x.Spaces!)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
            
            if(result == null)
                throw new ParklaConcurrentDeletionException("The park area trying to delete is already deleted by another user");
            context.Remove(result);

            var resUsers = result.Pricings!.SelectMany(x => x.Reservations!.Select(y => new {
                Reservation = y,
                User = y.User!
            }));

            foreach (var resUser in resUsers) {
                var reservation = resUser.Reservation;
                var timeIntervalAsHour = reservation.EndTime!.Value.Subtract(reservation.StartTime!.Value).TotalHours;
                reservation.User!.Wallet += Pricing.GetPricePerHour(reservation.Pricing!) * (float)timeIntervalAsHour;
            }

            var emptySpaces = 0;
            var reservedSpaces = 0;
            var occupiedSpaces = 0;

            foreach (var space in result.Spaces!) {
                switch(space.Status) {
                    case SpaceStatus.EMPTY:
                        emptySpaces++;
                        break;
                    case SpaceStatus.RESERVED:
                        reservedSpaces++;
                        break;
                    case SpaceStatus.OCCUPIED:
                        occupiedSpaces++;
                        break;
                }
            }

            // restore changes before
            var parkOriginals = (Park)context.Entry(park).OriginalValues.Clone().ToObject();
            park.EmptySpace = parkOriginals.EmptySpace;
            park.ReservedSpace = parkOriginals.ReservedSpace;
            park.OccupiedSpace = parkOriginals.OccupiedSpace;

            park.EmptySpace -= emptySpaces;
            park.ReservedSpace -= reservedSpaces;
            park.OccupiedSpace -= occupiedSpaces;
            
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return false;
        },
        async (err) => {
            var entry = err.Entries.Single();

            if(entry.Entity is Pricing pri) 
            {
                entry.State = EntityState.Detached;
            }
            else if(entry.Entity is Park) 
            {
                var changed = await ReloadParkCheckMinAvgMax(entry, cancellationToken).ConfigureAwait(false);
                if(changed) 
                    (park.MinPrice, park.AvaragePrice, park.MaxPrice) = await FindNewParkMinAvgMaxAsync(context, area, cancellationToken).ConfigureAwait(false);
            }
            else if(entry.Entity is User user) {
                await entry.ReloadAsync(cancellationToken).ConfigureAwait(false);
            }
            else if(entry.Entity is ParkArea area) {
                var dbval = await entry.GetDatabaseValuesAsync(cancellationToken).ConfigureAwait(false);
                if(dbval == null)
                    throw new ParklaConcurrentDeletionException("The park area trying to delete is already deleted by another user");
                SetXMin(entry, dbval);
            }

            return true;
        }, cancellationToken).ConfigureAwait(false);

        if(cancelled) return null;
        return park; //////////////////////////////////////// SEND PARK AREA WITH SIGNALR AS DELETED. USERS MAY WATCHIN PARK SPACE CHANGES SO UI NEED TO BE CATCH THIS 
    }

    public async Task<ParkArea> UpdateTemplateAsync(
        ParkArea area, 
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
        var entry = context.Attach(area);
        var area2 = context.Set<ParkArea>()
            .Include(x => x.Spaces!)
            .ThenInclude(x => x.RealSpace);

        entry.Property(x => x.TemplateImage).IsModified = true;

        foreach (var space in area.Spaces!) {
            var eSpace = context.Entry(space);
            eSpace.State = space.Id == null ? EntityState.Added : EntityState.Modified;
            if(space.RealSpaceId != null) {

            }
        }

        var cancelled = await RetryOnConcurrencyErrorAsync(async () => {
            var deletedSpaces = context.Set<ParkSpace>()
                .Where(x => x.AreaId == area.Id && !area.Spaces!.Contains(x));
            context.RemoveRange(deletedSpaces);

            await context.SaveChangesAsync(cancellationToken);
            return false;
        },
        async (err) => {
            var entry = err.Entries.Single();
            var dbval = await entry.GetDatabaseValuesAsync(cancellationToken).ConfigureAwait(false);
            
            if(entry.Entity is ParkArea area) {
                if(dbval == null)
                    throw new ParklaException("The updating area was deleted by another user.", HttpStatusCode.NotFound);
                
                SetXMin(entry, dbval);
                return true;
            }

            

            return true;
        }, cancellationToken).ConfigureAwait(false);

        return area;
    }
}