using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Parkla.Core.Entities;
using Parkla.Core.Enums;
using Parkla.Core.Exceptions;
using Parkla.Core.Helpers;
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

    public virtual async Task<PagedList<ParkArea>> GetParkAreaPage(
        int nextRecord, 
        int pageSize, 
        Expression<Func<ParkArea, bool>>? filter = null,
        Expression<Func<ParkArea, object>>? orderBy = null,
        bool asc = true,
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
        IQueryable<ParkArea> resultSet;
        if(filter == null)
            resultSet = context.Set<ParkArea>().AsNoTracking();
        else
            resultSet = context.Set<ParkArea>().AsNoTracking().Where(filter);

        if(orderBy != null)
            if(asc)
                resultSet = resultSet.OrderBy(orderBy);
            else
                resultSet = resultSet.OrderByDescending(orderBy);

        var result = await ToPagedListAsync(resultSet, nextRecord, pageSize, cancellationToken).ConfigureAwait(false);
        return result;
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

    private static async Task RetryDeletedSpacesAsync(
        TContext context,
        ParkArea area,
        List<ParkSpace> deletingSpaces,
        List<User> updatingUsersWallet,
        CancellationToken cancellationToken = default
    ) {
        foreach (var item in deletingSpaces)
            context.Entry(item).State = EntityState.Detached;
        foreach (var item in updatingUsersWallet)
            context.Entry(item).State = EntityState.Detached;
        deletingSpaces.Clear();
        updatingUsersWallet.Clear();
        
        var newDeletingSpaces = await context.Set<ParkSpace>()
            .Where(x => x.AreaId == area.Id && !area.Spaces!.Contains(x))
            .Include(x => x.Reservations!)
            .ThenInclude(x => x.User)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);         
        context.RemoveRange(newDeletingSpaces);

        var resUsers = newDeletingSpaces.SelectMany(x => x.Reservations!.Select(y => new {
            Reservation = y,
            User = y.User!
        }));

        foreach (var resUser in resUsers) {
            var reservation = resUser.Reservation;
            var timeIntervalAsHour = reservation.EndTime!.Value.Subtract(reservation.StartTime!.Value).TotalHours;
            reservation.User!.Wallet += Pricing.GetPricePerHour(reservation.Pricing!) * (float)timeIntervalAsHour;
        }

        var newUpdatingUsersWallet = resUsers.Select(x => x.User).ToList();

        deletingSpaces.AddRange(newDeletingSpaces);
        updatingUsersWallet.AddRange(newUpdatingUsersWallet);
    }

    public new async Task<Tuple<ParkArea, Park?>> UpdateAsync(
        ParkArea area,
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
        var result = context.Update(area); // pricings are modified state with all props

        result.State = EntityState.Unchanged; // only 3 property of the area modified
        result.Property(x => x.Name).IsModified = true;
        result.Property(x => x.Description).IsModified = true;
        result.Property(x => x.ReservationsEnabled).IsModified = true;
            
        var park = await InitParkAndAreaMinAvgMax(context, area, cancellationToken).ConfigureAwait(false); // min avg max props modified of park and area
        
        var deletingPricings = new List<Pricing>();
        var updatingUsersWallet = new List<User>();

        var cancelled = await RetryOnConcurrencyErrorAsync(async () => 
        {
            await RetryDeletedPricingsAsync(context, area, deletingPricings, updatingUsersWallet, cancellationToken).ConfigureAwait(false); // area.pricings.[users.wallet] props modified
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
                var changed = await ReloadParkCheckMinAvgMax(entry, cancellationToken).ConfigureAwait(false); // reloads but not modify park so park state unhanged
                if(changed) 
                    (park.MinPrice, park.AvaragePrice, park.MaxPrice) = await FindNewParkMinAvgMaxAsync(context, area, cancellationToken).ConfigureAwait(false); // park min avg max modified
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
                SetXMin(entry, dbval); // same area values there because no reload occurs. xmin is set and at this point so every prop of area except statusupdatetime, empty-reserved-occupied spces props
            }
            return true;
        }, cancellationToken).ConfigureAwait(false);

        if(cancelled) return new(area, null);
        return new(area, park);
    }


    public override async Task<Tuple<ParkArea?,Park?>> DeleteAsync(ParkArea area, CancellationToken cancellationToken = default)
    {
        using var context = new TContext();

        var park = await InitParkAndAreaMinAvgMax(context, new ParkArea(){
            Id = area.Id,
            ParkId = area.ParkId,
            Pricings = Array.Empty<Pricing>()
        }, cancellationToken).ConfigureAwait(false);

        ParkArea? result = null; 

        var cancelled = await RetryOnConcurrencyErrorAsync(async () => 
        {
            result = await context.Set<ParkArea>().Where(x => x.Id == area.Id)
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

            // restore changes if park reloaded in concurrency error new db values restored or first values of park when this function was started
            var parkOriginals = (Park)context.Entry(park).OriginalValues.Clone().ToObject();
            park.EmptySpace = parkOriginals.EmptySpace;
            park.OccupiedSpace = parkOriginals.OccupiedSpace;

            foreach (var space in result.Spaces!) {
                switch(space.Status) {
                    case SpaceStatus.EMPTY:
                        park.EmptySpace--;
                        break;
                    case SpaceStatus.OCCUPIED:
                        park.OccupiedSpace--;
                        break;
                }
            }

            //new empty reserved occupied space values passed. If they are not changed, it is not problem to update time because at this time the values are correct.
            //this value different from park space value because park space values represents real park space update time
            park.StatusUpdateTime = DateTime.UtcNow;
            
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

        if(cancelled) return new(result, null);
        return new(result, park);
    }

    public async Task<Tuple<ParkArea,Park?>> UpdateTemplateAsync(
        ParkArea area, 
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
        var spaces = area.Spaces!.AsEnumerable(); // these values come from user so last state of database will be this. user wins strategy.
        
        foreach (var item in spaces) { // these relations will not be updated
            item.RealSpace = null;
            item.Reservations = null;
            item.ReceivedSpaceStatuses = null;
        }

        area.Spaces = Array.Empty<ParkSpace>();
        var result = context.Attach(area); // makes sure only template image is modified
        result.Property(x => x.TemplateImage).IsModified = true;
        
        var cancelled = await RetryOnConcurrencyErrorAsync(async () => 
        {
            await result.Reference(x => x.Park).LoadAsync(cancellationToken).ConfigureAwait(false); // load necessary references
            await result.Collection(x => x.Spaces!)
                .Query()
                .Include(x => x.RealSpace)
                .Include(x => x.Reservations!)
                .ThenInclude(x => x.User)
                .LoadAsync(cancellationToken)
                .ConfigureAwait(false);

            var addingSpaces = spaces.Where(x => !area.Spaces.Any(y => y.Id == x.Id)).ToList();
            foreach (var item in addingSpaces)
                item.Id = null; // if space does not exist in area.spaces that means it also does not exist in database so mark them as adding (after updaterange)
            
            var deletingSpaces = area.Spaces.Where(x => !spaces.Any(y => y.Id == x.Id)).ToList();
            foreach (var item in deletingSpaces)
                context.Remove(item); // updated park area doesn't have these db entities so they will be deleted. User wins strategy applied

            context.UpdateRange(spaces); // updates(and adds null id ones). other spaces was marked as deleted state.

            foreach (var item in spaces) {
                item.Status = item.RealSpace!.Status;
                item.StatusUpdateTime = item.RealSpace.StatusUpdateTime;
            }

            
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

        if(cancelled) return new(area, null);
        return new(area, area.Park);
    }
}