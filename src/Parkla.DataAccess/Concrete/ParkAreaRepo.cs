using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Parkla.Core.Entities;
using Parkla.Core.Enums;
using Parkla.Core.Exceptions;
using Parkla.Core.Helpers;
using Parkla.Core.Models;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class ParkAreaRepo<TContext> : EntityRepoBase<ParkArea, TContext>, IParkAreaRepo 
    where TContext: DbContext, new()
{
    public async Task<PagedList<InstantParkAreaReservedSpace>> GetParkAreaPage(
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
        
        resultSet = resultSet.Include(x => x.Spaces!)
            .ThenInclude(x => x.Reservations);

        if(orderBy != null)
            if(asc)
                resultSet = resultSet.OrderBy(orderBy);
            else
                resultSet = resultSet.OrderByDescending(orderBy);

        var count = resultSet.Count();
        var items = await resultSet.Skip(nextRecord)
            .Take(pageSize)
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
            
        var result = items.Select(x => {
            var obj = new InstantParkAreaReservedSpace(
                x,
                x.Spaces!.Sum(
                    y => y.Reservations!.Count)
            );
            x.Spaces = null;
            return obj;
        }).ToList();

        return new PagedList<InstantParkAreaReservedSpace>(result, nextRecord, pageSize, count);
    }

    private static readonly ParklaException _parkNotFound = new("Park of the area is deleted so also this area does not exist in database.", HttpStatusCode.BadRequest);

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

    public new async Task<Tuple<ParkArea, Park?>> UpdateAsync(
        ParkArea area,
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
            
        var park = await InitParkAndAreaMinAvgMax(context, area, cancellationToken).ConfigureAwait(false); // min avg max props modified of park and area
        context.Entry(park).State = EntityState.Detached;

        var parkClone = park;// these are states and every turn it can be changed or not. So operation will be changed via them
        var areaClone = area;// area has concurrency token so xmin will be updated. The state is xmin. 
        // For park minavgmax operation costs performance(scan for all parkareas to calculate) too much. Its state is minavgmax and never calculated if db park value is not changed. 

        var cancelled = await RetryOnConcurrencyErrorAsync(async () => 
        {
            var result = context.Update(areaClone); // pricings are modified state with all props
            var area = result.Entity;
            result.State = EntityState.Unchanged; // only 3 property of the area modified bu user
            result.Property(x => x.Name).IsModified = true;
            result.Property(x => x.Description).IsModified = true;
            result.Property(x => x.ReservationsEnabled).IsModified = true;
            result.Property(x => x.MinPrice).IsModified = true;
            result.Property(x => x.AvaragePrice).IsModified = true;
            result.Property(x => x.MaxPrice).IsModified = true;

            areaClone = (ParkArea)result.CurrentValues.Clone().ToObject();
            areaClone.Pricings = result.Entity.Pricings.Select(x => (Pricing)context.Entry(x).CurrentValues.Clone().ToObject()).ToList();
            parkClone = (Park)context.Update(park).CurrentValues.Clone().ToObject();

            var userPricings = new List<Pricing>(area.Pricings);
            
            var allPricings = await result.Collection(x => x.Pricings!)
                .Query()
                .Include(x => x.Reservations!)
                .ThenInclude(x => x.User)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            
            var deletingPricings = allPricings.Except(userPricings);
            foreach (var item in deletingPricings)
                context.Remove(item); // updated park area doesn't have these db entities so they will be deleted. User wins strategy applied

            foreach (var item in userPricings)
                context.Entry(item).State = EntityState.Detached;
            
            var someOrAllOfUserPricings = await context.Set<Pricing>()
                .Where(x => x.AreaId == area.Id && userPricings.Select(x => x.Id).Contains(x.Id))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (var item in userPricings) {
                var dbitem = someOrAllOfUserPricings.Where(x => x.Id == item.Id).FirstOrDefault(); 
                if(dbitem != null)
                {
                    context.Entry(dbitem).State = EntityState.Detached;
                    //context.Update(item); same thing with add
                    context.Attach(item).State = EntityState.Added; 
                    // these are also in database so update them
                }
                else {
                    item.Id = null; // database does not have these items because they are not in someorallofuserspaces retrieved from database
                    // dont use context.Add(item) because also relations becomes added state so singular state change
                    context.Entry(item).State = EntityState.Added;
                }
            }

            area.Pricings = userPricings;

            var resUsers = deletingPricings.SelectMany(x => x.Reservations!.Select(y => new {
                Reservation = y,
                User = y.User!
            }));

            foreach (var resUser in resUsers) {
                var reservation = resUser.Reservation;
                var timeIntervalAsHour = reservation.EndTime!.Value.Subtract(reservation.StartTime!.Value).TotalHours;
                reservation.User!.Wallet += Pricing.GetPricePerHour(reservation.Pricing!) * (float)timeIntervalAsHour;
            }

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return false;
        }, 
        async (err) => 
        {
            var entry = err.Entries.Single();

            if(entry.Entity is Park) 
            {
                var changed = await ReloadParkCheckMinAvgMax(entry, cancellationToken).ConfigureAwait(false); // reloads but not modify park so park state unhanged
                if(changed) 
                    (parkClone.MinPrice, parkClone.AvaragePrice, parkClone.MaxPrice) = await FindNewParkMinAvgMaxAsync(context, areaClone, cancellationToken).ConfigureAwait(false); // park min avg max modified
            }
            else if(entry.Entity is ParkArea entity) {
                // update or delete opreation but it is update because this function called on area update request
                // there is concurrency token for area so either version mismatch or record not found
                await entry.ReloadAsync(cancellationToken).ConfigureAwait(false);

                if(entry.State == EntityState.Detached)
                    throw new ParklaConcurrentDeletionException("The park area trying to update is already updated by another user");
                
                entity.Name = areaClone.Name; // these are modified columns
                entity.Description = areaClone.Description; // so apply these to the new park area with new version - last in wins
                entity.ReservationsEnabled = areaClone.ReservationsEnabled;
                entity.Pricings = areaClone.Pricings;
                entity.MinPrice = areaClone.MinPrice;
                entity.MaxPrice = areaClone.MaxPrice;
                entity.AvaragePrice = areaClone.AvaragePrice;
                //entity.Spaces remove relations because may they have another realtions can cause attach an entity twice
                //entity.spaces not used here but for exmaple assume entity.spaces[0].pricings[0].Id = 2
                //also assume entity.pricings[0] = 2. both is different object in memory because areaClone is clone and its pricings assigned
                //to entity.pricings. but entity.spaces are in context so entity.pricings[0] out of context and other one is in context.
                //in next rounds they will be in new areaClone and cause error.

                areaClone = entity; // entity will be detached after clear so it is like a clone now with new version and user modifies
            }

            context.ChangeTracker.Clear();
            return true;
        }, cancellationToken).ConfigureAwait(false);

        if(cancelled) return new(areaClone, null);

        var areaResult = context.Find<ParkArea>(areaClone.Id)!;
        var parkResult = context.Find<Park>(parkClone.Id)!;
        
        return new(areaResult, parkResult);
    }

    public override async Task<Tuple<ParkArea?,Park?>> DeleteAsync(ParkArea area, CancellationToken cancellationToken = default)
    {
        using var context = new TContext();
        area = new(){
            Id = area.Id,
            ParkId = area.ParkId,
            Pricings = Array.Empty<Pricing>()
        };
        var park = await InitParkAndAreaMinAvgMax(context, area, cancellationToken).ConfigureAwait(false);
        context.Entry(park).State = EntityState.Detached;

        ParkArea areaClone = area;
        Park parkClone = park;
        var cancelled = await RetryOnConcurrencyErrorAsync(async () => 
        {
            parkClone = (Park)context.Update(parkClone).CurrentValues.Clone().ToObject();
            
            var result = await context.Set<ParkArea>().Where(x => x.Id == area.Id)
                .Include(x => x.Pricings!)
                .ThenInclude(x => x.Reservations!)
                .ThenInclude(x => x.User)
                .Include(x => x.Spaces!)
                .SingleOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);
            
            if(result == null)
                throw new ParklaConcurrentDeletionException("The park area trying to delete is already deleted by another user");
            
            areaClone = (ParkArea)context.Entry(areaClone).CurrentValues.Clone().ToObject();
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

            //new empty reserved occupied space values passed but
            //this will not be updated because this value means when a realspace data arrived to this park at last
            //park.StatusUpdateTime = DateTime.UtcNow;
            
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return false;
        },
        async (err) => {
            var entry = err.Entries.Single();

            if(entry.Entity is Park) 
            {
                var changed = await ReloadParkCheckMinAvgMax(entry, cancellationToken).ConfigureAwait(false);
                if(changed) 
                    (parkClone.MinPrice, parkClone.AvaragePrice, parkClone.MaxPrice) = await FindNewParkMinAvgMaxAsync(context, areaClone, cancellationToken).ConfigureAwait(false);
            }

            context.ChangeTracker.Clear();
            return true;
        }, cancellationToken).ConfigureAwait(false);

        if(cancelled) return new(areaClone, null);

        var areaResult = context.Find<ParkArea>(areaClone.Id)!;
        var parkResult = context.Find<Park>(parkClone.Id)!;
        return new(areaResult, parkResult);
    }

    public async Task<Tuple<ParkArea,Park?>> UpdateTemplateAsync(
        ParkArea area, 
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
        ParkArea areaClone = area; 
        
        var cancelled = await RetryOnConcurrencyErrorAsync(async () => 
        {
            var result = context.Attach(areaClone);
            var area = result.Entity;
            result.Property(x => x.TemplateImage).IsModified = true;
            result.Collection(x => x.Spaces).IsModified = true;

            areaClone = (ParkArea)result.CurrentValues.Clone().ToObject(); // every turn everything is redone
            areaClone.Spaces = result.Entity.Spaces.Select(x => (ParkSpace)context.Entry(x).CurrentValues.Clone().ToObject()).ToList();
            
            await result.Reference(x => x.Park).LoadAsync(cancellationToken).ConfigureAwait(false);

            if(area.Park == null)
                throw _parkNotFound;

            var userSpaces = new List<ParkSpace>(area.Spaces); // every round it is userSpaces because in catch block whole state cleared like function started
            
            var allSpaces = await result.Collection(x => x.Spaces!)
                .Query()
                .Include(x => x.Reservations!)
                .ThenInclude(x => x.User)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
            
            var deletingSpaces = allSpaces.Except(userSpaces);
            foreach (var item in deletingSpaces)
                context.Remove(item); // updated park area doesn't have these db entities so they will be deleted. User wins strategy applied
            
            foreach (var item in userSpaces)
                context.Entry(item).State = EntityState.Detached;
            
            var someOrAllOfUserSpaces = await context.Set<ParkSpace>()
                .Where(x => x.AreaId == area.Id && userSpaces.Select(x => x.Id).Contains(x.Id))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (var item in userSpaces) {
                var dbitem = someOrAllOfUserSpaces.Where(x => x.Id == item.Id).FirstOrDefault(); 
                if(dbitem != null)
                {
                    var eDbItem = context.Entry(dbitem);
                    eDbItem.State = EntityState.Detached;
                    //context.Update(item); dont use update. Same thing with context.add below or above
                    var eItem = context.Attach(item); 
                    item.xmin = dbitem.xmin; // this is overridden because last in wins for spaces.
                    // in catch all of the user spaces can be reloaded and xmin values can be updated but they are loaded right here already.

                    //IF AN UPDATING ENTITY HAS COMPLETELY SAME VALUES FOR ALL PROPERTIES(COLUMNS) IT CAUSES CONCURRENCY EXCEPTION
                    //IT IS SUPPOSED TO BE MODIFIED BUT EVERYTHING IS SAME SO EXPECTED 1 ROW EFFECTED BUT NO EFFECT. COMPARE PROPERTIES AND IF ALL EQUALS MAKE UNCHANGED STATE
                    if(item.Equals(dbitem))
                        eItem.State = EntityState.Unchanged;
                    else
                        eItem.State = EntityState.Modified;
                }
                else {
                    item.Id = null; // database does not have these items because they are not in someorallofuserspaces retrieved from database
                    // dont use context.Add(item) because also realspace becomes added state so singular state change
                    context.Entry(item).State = EntityState.Added;
                }
            }

            area.Spaces = userSpaces;
            
            var realSpaces = await context.Set<RealParkSpace>()
                .Where(x => area.Spaces.Select(x => x.RealSpaceId).Contains(x.Id) || area.Spaces.Select(x => x.Id).Contains(x.SpaceId))
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            foreach (var item in userSpaces) { 
                // ALL SPACES HAS A REALSPACE ID INITALLY AND IT IS A MUST FIND THE REALSPACE OF THE SPACE 
                if(item.RealSpace == null) {
                    var realSpace = realSpaces
                        .Where(x => x.Id == item.RealSpaceId)
                        .FirstOrDefault();
                    
                    if(realSpace == null)
                        throw new ParklaException($"One of the spaces with {item.Name} name has had binded to RealSpace which is not exist in database. Please select another one again", HttpStatusCode.BadRequest);
                    
                    var localOldRealSpace = context.Set<RealParkSpace>()
                        .Local
                        .Where(x => x.SpaceId == item.Id)
                        .FirstOrDefault();
                    
                    if(localOldRealSpace != null && localOldRealSpace.SpaceId == item.Id) {
                        localOldRealSpace.SpaceId = null;
                        // maybe locally this space binded another space so fetching database may overwrite it
                    } else {
                        var oldRealSpace = await context.Set<RealParkSpace>()
                            .Where(x => x.SpaceId == item.Id)
                            .FirstOrDefaultAsync(cancellationToken)
                            .ConfigureAwait(false);
                        
                        if(oldRealSpace != null && oldRealSpace.Space == item) 
                            oldRealSpace.SpaceId = null;
                    }
                    

                    if(realSpace.SpaceId == null) {
                        item.RealSpace = realSpace;
                    }
                    else {
                        //if realspace was binded before update is allowed only and only if it is binded changing ones.
                        //Because other spaces is not changing right now and that means to unbind a realspace the realspaceid of the binded space must set null
                        //or another available space. Also, this is an update operation and only userSpaces are changing right now so if realspace was not binded
                        //one of them that means it cant be changed right now. 
                        var oldSpace = userSpaces.Where(x => x.Id == realSpace.SpaceId).FirstOrDefault();
                        if(oldSpace != null && oldSpace.RealSpaceId != realSpace.Id) {
                            //oldspace exists which is changing and it is not binded this realspace any more. So this realspace is available
                            item.RealSpace = realSpace;
                        }
                        else throw new ParklaException($"One of the spaces with {item.Name} name has had binded to RealSpace which has already binded to another space. Please select another one again", HttpStatusCode.BadRequest);
                    }
                }   

                if(item.Status != item.RealSpace.Status) {
                    switch(item.Status) {
                        case SpaceStatus.EMPTY:
                            area.EmptySpace--;
                            area.Park.EmptySpace--;
                            break;
                        case SpaceStatus.OCCUPIED:
                            area.Park.OccupiedSpace--;
                            area.OccupiedSpace--;
                            break;
                    }

                    switch(item.RealSpace.Status) {
                        case SpaceStatus.EMPTY:
                            area.Park.EmptySpace++;
                            area.EmptySpace++;
                            break;
                        case SpaceStatus.OCCUPIED:
                            area.Park.OccupiedSpace++;
                            area.OccupiedSpace++;
                            break;
                    }
                }

                item.Status = item.RealSpace.Status;
                item.StatusUpdateTime = item.RealSpace.StatusUpdateTime;
            }

            //dont update like delete operation of the area
            //area.Park.StatusUpdateTime = DateTime.UtcNow;

            // these ones will be deleted also reservations will be deleted. So give the money of the user which paid for reservation.
            foreach (var deletingSpace in deletingSpaces) {
                foreach (var reservation in deletingSpace.Reservations!){
                    var timeIntervalAsHour = reservation.EndTime!.Value.Subtract(reservation.StartTime!.Value).TotalHours;
                    reservation.User!.Wallet += Pricing.GetPricePerHour(reservation.Pricing!) * (float)timeIntervalAsHour;
                }
            }
            
            await context.SaveChangesAsync(cancellationToken);
            return false;
        },
        async (err) => {
            var entry = err.Entries.Single();
            await entry.ReloadAsync(cancellationToken).ConfigureAwait(false);

            if(entry.Entity is ParkArea entity) {
                if(entry.State == EntityState.Detached)
                    throw new ParklaConcurrentDeletionException("The park area trying to update is already deleted by another user");
                
                entity.Park = null;
                entity.TemplateImage = areaClone.TemplateImage; // these are modified columns
                entity.Spaces = areaClone.Spaces; // so apply these to the new park area with new version - last in wins

                areaClone = entity; // entity will be detached after clear so it is like a clone now with new version and user modifies
            }
            else if(entry.Entity is ParkSpace spaceEntity) {
                // spaces have concurrency token and a state in areaClone so need to be updated if version changes
                // it is necessary to update xmin values of this spaces but
                // in try block it is handled. So next round they will be updated.
            }
            
            context.ChangeTracker.Clear();
            return true;
        }, cancellationToken).ConfigureAwait(false);

        if(cancelled) return new(areaClone, null);
        
        var areaResult = context.Find<ParkArea>(areaClone.Id)!;
        return new(areaResult, areaResult.Park);
    }
}