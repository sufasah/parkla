using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
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
    private static readonly ParklaException _parkNotFound = new("Park of the area was deleted so also this area does not exist in database.", HttpStatusCode.BadRequest);
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
            .GroupBy(x => x.Id)
            .Select(g => new {
                ParkArea = g.First(x => x.Id == g.Key),
                ReservedSpaceCount = g.Sum(
                    x => x.Spaces.Sum(
                        y => y.Reservations!.Any(t => t.EndTime > DateTime.UtcNow && t.EndTime < DateTime.UtcNow.AddDays(1).Date) ? 1 : 0))
            })
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);
            
        var result = items.Select(x => {
            x.ParkArea.Spaces = null!;
            return new InstantParkAreaReservedSpace(x.ParkArea, x.ReservedSpaceCount);
        }).ToList();

        return new PagedList<InstantParkAreaReservedSpace>(result, nextRecord, pageSize, count);
    }

    private static async Task<Tuple<float?,float?,float?>> FindNewParkMinAvgMaxAsync(
        TContext context,
        Guid parkId,
        int areaId,
        float? newAreaMin,
        float? newAreaAverage,
        float? newAreaMax,
        float newAreaCount,
        CancellationToken cancellationToken = default
    ) {
        var mmaResult = await context.Set<ParkSpace>()
            .AsNoTracking()
            .Include(x => x.Pricing)
            .Include(x => x.Area)
            .Where(x => x.Area!.ParkId == parkId && x.AreaId != areaId && x.Pricing != null)
            .GroupBy(_ => 1)
            .Select(x => new {
                Count = x.Count(),
                Min = x.Min(
                    y => y.Pricing!.Price 
                    * (TimeUnit.MINUTE == y.Pricing!.Unit ? 60 : 1)
                    / (TimeUnit.DAY == y.Pricing!.Unit ? 24 : 1)
                    / (TimeUnit.MONTH == y.Pricing!.Unit ? 720 : 1)
                    / y.Pricing!.Amount
                ),
                Average = x.Average(
                    y => y.Pricing!.Price
                    * (TimeUnit.MINUTE == y.Pricing!.Unit ? 60 : 1)
                    / (TimeUnit.DAY == y.Pricing!.Unit ? 24 : 1)
                    / (TimeUnit.MONTH == y.Pricing!.Unit ? 720 : 1)
                    / y.Pricing!.Amount
                ),
                Max = x.Max(
                    y => y.Pricing!.Price
                    * (TimeUnit.MINUTE == y.Pricing!.Unit ? 60 : 1)
                    / (TimeUnit.DAY == y.Pricing!.Unit ? 24 : 1)
                    / (TimeUnit.MONTH == y.Pricing!.Unit ? 720 : 1)
                    / y.Pricing!.Amount
                )
            })
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);

        if(mmaResult == null)
            return new(newAreaMin, newAreaAverage, newAreaMax);
            
        var newMin = mmaResult.Min;
        var newAverage = mmaResult.Average;
        var newMax = mmaResult.Max;
        var count = (float) mmaResult.Count;
        
        if(newAreaAverage != null)
        newAverage = count > 0 && newAverage != null 
            ? newAverage * (count / (count + newAreaCount)) + newAreaAverage * (newAreaCount / (count + newAreaCount))
            : newAreaAverage;
                
        if(newAreaMin != null)
        newMin = newMin != null
            ? Math.Min((float)newMin, (float)newAreaMin)
            : newAreaMin;

        if(newAreaMax != null)
        newMax = newMax != null
            ? Math.Max((float)newMax, (float)newAreaMax)
            : newAreaMax;
        
        return new(newMin, newAverage, newMax);
    }
    
    private static Tuple<float?,float?,float?,float> FindPricingsMinAvgMaxCount(
        IEnumerable<Pricing> pricings
    ) {
        var mmaResult = pricings.GroupBy(_ => 1)
            .Select(x => new {
                Count = x.Count(),
                Min = x.Min(y => Pricing.GetPricePerHour(y)),
                Average = x.Average(y => Pricing.GetPricePerHour(y)),
                Max = x.Max(y => Pricing.GetPricePerHour(y))
            })
            .FirstOrDefault();

        
        if(mmaResult == null)
            return new(null, null, null, 0f);
            
        var newMin = mmaResult.Min;
        var newAverage = mmaResult.Average;
        var newMax = mmaResult.Max;
        var newCount = (float) mmaResult.Count;
        
        return new(newMin, newAverage, newMax, newCount);
    }

    public new async Task<ParkArea> AddAsync(ParkArea area, CancellationToken cancellationToken = default) {
        using var context = new TContext();

        var areaClone = area;
        while(!cancellationToken.IsCancellationRequested) {
            try {
                var result = context.Add(area);
                areaClone = (ParkArea)result.CurrentValues.Clone().ToObject();

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return area;
            }
            catch(DbUpdateConcurrencyException err)
            {
                var entry = err.Entries.Single();
                context.ChangeTracker.Clear();
            }
        }

        return areaClone;
    }

    public new async Task<Tuple<ParkArea, Park?, IEnumerable<Reservation>>> UpdateAsync(
        ParkArea areaParam,
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
            
        var areaClone = areaParam;
        while(!cancellationToken.IsCancellationRequested) {
            try {
                var result = context.Update(areaClone);
                var area = result.Entity;
                areaClone = (ParkArea)result.CurrentValues.Clone().ToObject();
                areaClone.Pricings = result.Entity.Pricings.Select(x => (Pricing)context.Entry(x).CurrentValues.Clone().ToObject()).ToList();

                result.State = EntityState.Unchanged; // only 3 property of the area modified bu user
                result.Property(x => x.Name).IsModified = true;
                result.Property(x => x.Description).IsModified = true;
                result.Property(x => x.ReservationsEnabled).IsModified = true;
                result.Property(x => x.MinPrice).IsModified = true;
                result.Property(x => x.AveragePrice).IsModified = true;
                result.Property(x => x.MaxPrice).IsModified = true;

                var userPricings = new List<Pricing>(area.Pricings);
                
                var allPricings = await result.Collection(x => x.Pricings!)
                    .Query()
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                
                var deletingPricings = allPricings.Except(userPricings);
                foreach (var item in deletingPricings) {
                    var dpEntry = context.Entry(item);
                    dpEntry.State = EntityState.Deleted; // updated park area doesn't have these db entities so they will be deleted. User wins strategy applied
                }

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
                        context.Attach(item).State = EntityState.Modified; 
                        // these are also in database so update them
                    }
                    else {
                        item.Id = null; // database does not have these items because they are not in someorallofuserspaces retrieved from database
                        // dont use context.Add(item) because also relations becomes added state so singular state change
                        context.Entry(item).State = EntityState.Added;
                    }
                }

                area.Pricings = userPricings;

                var spaces = await context.Set<ParkSpace>()
                    .Where(x => x.AreaId == area.Id && x.PricingId != null && !deletingPricings.Select(y => y.Id).Contains(x.PricingId))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                var (newAreaMin, newAreaAverage, newAreaMax, newAreaCount) = FindPricingsMinAvgMaxCount(spaces.Select(x => x.Pricing!));
                area.MinPrice = newAreaMin;
                area.AveragePrice = newAreaAverage;
                area.MaxPrice = newAreaMax;
                
                var park = await context.FindAsync<Park>(new object[]{area.ParkId!}, cancellationToken: cancellationToken).ConfigureAwait(false);

                if(park == null)
                    throw _parkNotFound;

                (area.Park!.MinPrice, area.Park.AveragePrice, area.Park.MaxPrice) = await FindNewParkMinAvgMaxAsync(
                    context,
                    area.ParkId!.Value,
                    area.Id!.Value,
                    newAreaMin,
                    newAreaAverage,
                    newAreaMax,
                    newAreaCount,
                    cancellationToken
                ).ConfigureAwait(false);

                var deletingSpaces = await context.Set<ParkSpace>()
                    .Where(x => x.AreaId == area.Id && deletingPricings.Select(y => y.Id).Contains(x.PricingId))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                var deletionTime = DateTime.UtcNow;
                var deletingSpaceIds = deletingSpaces.Select(y => y.Id);
                var deletingReservations = await context.Set<Reservation>()
                    .Include(x => x.User)
                    .Where(x => 
                        x.EndTime > deletionTime &&
                        deletingSpaceIds.Contains(x.SpaceId))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                foreach (var reservation in deletingReservations) {
                    context.Entry(reservation).State = EntityState.Deleted;
                    var timeIntervalAsHour = reservation.EndTime!.Value.Subtract(deletionTime > reservation.StartTime ? deletionTime : reservation.StartTime!.Value).TotalHours;
                    reservation.User!.Wallet += Pricing.GetPricePerHour(reservation.Space!.Pricing!) * (float)timeIntervalAsHour;
                }

                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);

                foreach (var pricing in area.Pricings)
                    pricing.Spaces = null!;

                return new(area, area.Park, deletingReservations);
            }
            catch(DbUpdateConcurrencyException err) {
                var entry = err.Entries.Single();

                if(entry.Entity is ParkArea entity) {
                    // update or delete opreation but it is update because this function called on area update request
                    // there is concurrency token for area so either version mismatch or record not found
                    await entry.ReloadAsync(cancellationToken).ConfigureAwait(false);

                    if(entry.State == EntityState.Detached)
                        throw new ParklaConcurrentDeletionException("The park area trying to update was deleted by another user");

                    areaClone.EmptySpace = entity.EmptySpace;
                    areaClone.OccupiedSpace = entity.OccupiedSpace;
                    areaClone.StatusUpdateTime = entity.StatusUpdateTime;
                    areaClone.TemplateImage = entity.TemplateImage;
                    areaClone.xmin = entity.xmin;
                    //entity.Spaces remove relations because may they have another realtions can cause attach an entity twice
                    //entity.spaces not used here but for exmaple assume entity.spaces[0].pricings[0].Id = 2
                    //also assume entity.pricings[0] = 2. both is different object in memory because areaClone is clone and its pricings assigned
                    //to entity.pricings. but entity.spaces are in context so entity.pricings[0] out of context and other one is in context.
                    //in next rounds they will be in new areaClone and cause error.
                }

                context.ChangeTracker.Clear();
            };
        }

        return new(areaClone, null, Array.Empty<Reservation>());
    }

    public override async Task<Tuple<ParkArea?, Park?, IEnumerable<ParkSpace>, IEnumerable<Reservation>>> DeleteAsync(ParkArea areaParam, CancellationToken cancellationToken = default)
    {
        using var context = new TContext();

        while(!cancellationToken.IsCancellationRequested) {
            try {
                var area = await context.Set<ParkArea>().Where(x => x.Id == areaParam.Id)
                    .Include(x => x.Pricings!)
                    .Include(x => x.Spaces!)
                    .Include(x => x.Park!)
                    .SingleOrDefaultAsync(cancellationToken)
                    .ConfigureAwait(false);
                
                if(area == null)
                    throw new ParklaConcurrentDeletionException("The park area trying to delete is already deleted by another user");
                
                context.Remove(area);

                var deletionTime = DateTime.UtcNow;
                var deletingPricingSpaceIds = area.Pricings.SelectMany(y => y.Spaces).Select(y => y.Id);
                var deletingReservations = await context.Set<Reservation>()
                    .Include(x => x.User)
                    .Include(x => x.Space)
                    .ThenInclude(x => x!.Pricing)
                    .Where(x => 
                        x.EndTime > deletionTime && (
                            (area.Spaces.Select(y => y.Id).Contains(x.SpaceId) && x.Space!.Pricing != null)  ||
                            deletingPricingSpaceIds.Contains(x.SpaceId)))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                foreach (var reservation in deletingReservations) {
                    context.Entry(reservation).State = EntityState.Deleted;
                    var timeIntervalAsHour = reservation.EndTime!.Value.Subtract(deletionTime > reservation.StartTime ? deletionTime : reservation.StartTime!.Value).TotalHours;
                    reservation.User!.Wallet += Pricing.GetPricePerHour(reservation.Space!.Pricing!) * (float)timeIntervalAsHour;
                }

                foreach (var space in area.Spaces!) {
                    switch(space.Status) {
                        case SpaceStatus.EMPTY:
                            area.Park!.EmptySpace--;
                            break;
                        case SpaceStatus.OCCUPIED:
                            area.Park!.OccupiedSpace--;
                            break;
                    }
                }
                
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return new(area, area.Park, area.Spaces, deletingReservations);
            }
            catch(DbUpdateConcurrencyException err) {
                var entry = err.Entries.Single();
                context.ChangeTracker.Clear();
            }
        }
        return new(null, null, Array.Empty<ParkSpace>(), Array.Empty<Reservation>());
    }

    public async Task<Tuple<ParkArea, Park?, IEnumerable<ParkSpace>, IEnumerable<Reservation>>> UpdateTemplateAsync(
        ParkArea areaParam, 
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();

        ParkArea areaClone = areaParam;

        while(!cancellationToken.IsCancellationRequested) {
            try {
                var result = context.Attach(areaClone);
                var area = result.Entity;
                result.Property(x => x.TemplateImage).IsModified = true;
                result.Collection(x => x.Spaces).IsModified = true;

                areaClone = (ParkArea)result.CurrentValues.Clone().ToObject(); // every turn everything is redone
                areaClone.Spaces = result.Entity.Spaces.Select(x => {
                    var spaceEntry = context.Entry(x);
                    var clone = (ParkSpace)spaceEntry.CurrentValues.Clone().ToObject();
                    if(x.Pricing != null) {
                        clone.Pricing = (Pricing)context.Entry(x.Pricing).CurrentValues.Clone().ToObject();
                    }
                    return clone;
                }).ToList();
                
                var park = await context.FindAsync<Park>(new object[]{area.ParkId!}, cancellationToken: cancellationToken).ConfigureAwait(false);
                
                if(park == null)
                    throw _parkNotFound;

                var userSpaces = new List<ParkSpace>(area.Spaces); // every round it is userSpaces because in catch block whole state cleared like function started
                
                var allSpaces = await result.Collection(x => x.Spaces!)
                    .Query()
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                
                var deletingSpaces = allSpaces.Except(userSpaces);
                foreach (var item in deletingSpaces) {
                    var dsEntry = context.Entry(item);
                    dsEntry.State = EntityState.Deleted;
                    
                    if(item.Status == SpaceStatus.EMPTY)
                        park.EmptySpace--;
                    else if(item.Status == SpaceStatus.OCCUPIED)
                        park.OccupiedSpace--;
                }
                
                foreach (var item in userSpaces)
                    context.Entry(item).State = EntityState.Detached;
                
                var someOrAllOfUserSpaces = await context.Set<ParkSpace>()
                    .Where(x => x.AreaId == area.Id && userSpaces.Select(x => x.Id).Contains(x.Id))
                    .Include(x => x.Pricing)
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                var spacesPricingSetNul = new List<ParkSpace>();

                foreach (var item in userSpaces) {
                    var dbitem = someOrAllOfUserSpaces.Where(x => x.Id == item.Id).FirstOrDefault(); 
                    if(dbitem != null)
                    {
                        var eDbItem = context.Entry(dbitem);
                        eDbItem.State = EntityState.Detached;
                        if(dbitem.Pricing != null) {
                            var eDbPricingItem = context.Entry(dbitem.Pricing);
                            eDbPricingItem.State = EntityState.Detached;
                        }
                        //context.Update(item); dont use update. Same thing with context.add below or above
                        var eItem = context.Entry(item); 
                        item.xmin = dbitem.xmin; // this is overridden because last in wins for spaces.
                        // in catch all of the user spaces can be reloaded and xmin values can be updated but they are loaded right here already.

                        //IF AN UPDATING ENTITY HAS COMPLETELY SAME VALUES FOR ALL PROPERTIES(COLUMNS) IT CAUSES CONCURRENCY EXCEPTION
                        //IT IS SUPPOSED TO BE MODIFIED BUT EVERYTHING IS SAME SO EXPECTED 1 ROW EFFECTED BUT NO EFFECT. COMPARE PROPERTIES AND IF ALL EQUALS MAKE UNCHANGED STATE
                        if(item.Equals(dbitem))
                            eItem.State = EntityState.Unchanged;
                        else {
                            eItem.State = EntityState.Modified;

                            if(dbitem.PricingId != null && item.PricingId == null) {
                                spacesPricingSetNul.Add(item);
                            }
                        }
                    }
                    else {
                        item.Id = null; // database does not have these items because they are not in someorallofuserspaces retrieved from database
                        // dont use context.Add(item) because also realspace becomes added state so singular state change
                        context.Entry(item).State = EntityState.Added;
                    }
                }

                area.Spaces = userSpaces;

                var userSpacePricings = await context.Set<Pricing>()
                    .Where(x => area.Spaces.Select(y => y.PricingId).Contains(x.Id))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);
                
                foreach(var item in area.Spaces) {
                    if(item.PricingId != null && item.Pricing == null)
                        throw new ParklaConcurrentDeletionException($"One of the spaces with '{item.Name}' name has a pricing which was deleted by another user. Please select the pricing again.");
                }

                var (newAreaMin, newAreaAverage, newAreaMax, newAreaCount) = FindPricingsMinAvgMaxCount(userSpacePricings);
                area.MinPrice = newAreaMin;
                area.AveragePrice = newAreaAverage;
                area.MaxPrice = newAreaMax;
                
                result.Property(x => x.MinPrice).IsModified = result.Property(x => x.AveragePrice).IsModified = result.Property(x => x.MaxPrice).IsModified = true;
                
                //var park = await context.FindAsync<Park>(new object[]{area.ParkId!}, cancellationToken: cancellationToken).ConfigureAwait(false);

                //if(park == null)
                //    throw _parkNotFound;

                (area.Park!.MinPrice, area.Park.AveragePrice, area.Park.MaxPrice) = await FindNewParkMinAvgMaxAsync(
                    context,
                    area.ParkId!.Value,
                    area.Id!.Value,
                    newAreaMin,
                    newAreaAverage,
                    newAreaMax,
                    newAreaCount,
                    cancellationToken
                ).ConfigureAwait(false);
                
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
                    } else {
                        // SPACES WANT TO BE BINDED TO REALSPACEID ONES WHICH IS DIFFERENT FROM REALSPACE PROP
                        // SO THIS ALGORITHM CAN BE LIKE THAT FIND NEXT ONE ASSIGN IT TO REALSPACE
                        item.RealSpace = await context.FindAsync<RealParkSpace>(new object[]{item.RealSpaceId!}, cancellationToken: cancellationToken).ConfigureAwait(false);
                        if(item.RealSpace == null)
                            throw new ParklaException($"One of the spaces with {item.Name} name has not binded to any RealSpace", HttpStatusCode.BadRequest);
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

                    item.Status = item.RealSpace!.Status;
                    item.StatusUpdateTime = item.RealSpace.StatusUpdateTime;
                }

                // some spaces will be deleted also reservations will be deleted. So give the money of the user which paid for reservation.
                var deletionTime = DateTime.UtcNow;
                var deletingReservations = await context.Set<Reservation>()
                    .AsNoTracking()
                    .Include(x => x.User)
                    .Include(x => x.Space)
                    .ThenInclude(x => x!.Pricing)
                    .Where(x => 
                        x.EndTime > deletionTime && 
                        (deletingSpaces.Select(y => y.Id).Contains(x.SpaceId) 
                         || spacesPricingSetNul.Select(y => y.Id).Contains(x.SpaceId)))
                    .ToListAsync(cancellationToken)
                    .ConfigureAwait(false);

                foreach (var reservation in deletingReservations) {
                    context.Entry(reservation).State = EntityState.Deleted;
                    
                    var lUser = context.Set<User>().Local.FirstOrDefault(x => x.Id == reservation.User!.Id);
                    if(lUser != null) reservation.User = lUser;
                    else context.Entry(reservation.User!).State = EntityState.Modified;
                    
                    var timeIntervalAsHour = reservation.EndTime!.Value.Subtract(deletionTime > reservation.StartTime ? deletionTime : reservation.StartTime!.Value).TotalHours;
                    reservation.User!.Wallet += Pricing.GetPricePerHour(reservation.Space!.Pricing!) * (float)timeIntervalAsHour;
                }
                
                await context.SaveChangesAsync(cancellationToken);
                return new(area, area.Park, deletingSpaces, deletingReservations);
            }
            catch(DbUpdateConcurrencyException err) {
                var entry = err.Entries.Single();
                await entry.ReloadAsync(cancellationToken).ConfigureAwait(false);

                if(entry.Entity is ParkArea entity) {
                    if(entry.State == EntityState.Detached)
                        throw new ParklaConcurrentDeletionException("The park area trying to update was deleted by another user");
                    
                    entity.Park = null;
                    entity.TemplateImage = areaClone.TemplateImage; // these are modified columns
                    entity.Spaces = areaClone.Spaces; // so apply these to the new park area with new version - last in wins
                    entity.Pricings = new List<Pricing>();

                    areaClone = entity; // entity will be detached after clear so it is like a clone now with new version and user modifies
                }
                else if(entry.Entity is ParkSpace spaceEntity) {
                    // spaces have concurrency token and a state in areaClone so need to be updated if version changes
                    // it is necessary to update xmin values of this spaces but
                    // in try block it is handled. So next round they will be updated.
                }
                
                context.ChangeTracker.Clear();
            }
        }

        return new(areaClone, null, Array.Empty<ParkSpace>(), Array.Empty<Reservation>());

    }

    public async Task<List<InstantParkAreaIdReservedSpace>> GetParkAreasReserverdSpaceCountAsync(int[] ids, CancellationToken cancellationToken)
    {
       using var context = new TContext();
        var result = await context.Set<ParkArea>()
            .Include(x => x.Spaces)
            .ThenInclude(x => x.Reservations)
            .GroupBy(x => x.Id)
            .Select(g => new {
                AreaId = g.Key,
                ReservedSpaceCount = g.Sum(
                    x => x.Spaces.Sum(
                        y => y.Reservations!.Any(t => t.EndTime > DateTime.UtcNow && t.EndTime < DateTime.UtcNow.AddDays(1).Date) ? 1 : 0))
            })
            .Where(x => ids.Any(y => y == x.AreaId!.Value))
            .ToListAsync(cancellationToken)
            .ConfigureAwait(false);

        return result.Select(x => new InstantParkAreaIdReservedSpace(x.AreaId!.Value,x.ReservedSpaceCount)).ToList();
    }

    public async Task<InstantParkAreaReservedSpace?> GetParkAreaAsync(
        Expression<Func<ParkArea, bool>>? filter = null, 
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

        var item = await resultSet
            .GroupBy(x => x.Id)
            .Select(g => new {
                ParkArea = g.First(x => x.Id == g.Key),
                ReservedSpaceCount = g.Sum(
                    x => x.Spaces.Sum(
                        y => y.Reservations!.Any(t => t.EndTime > DateTime.UtcNow && t.EndTime < DateTime.UtcNow.AddDays(1).Date) ? 1 : 0))
            })
            .FirstOrDefaultAsync(cancellationToken)
            .ConfigureAwait(false);
        
        if(item == null || item.ParkArea == null) return null;
        item.ParkArea.Spaces = null!;
        return new InstantParkAreaReservedSpace(item.ParkArea, item.ReservedSpaceCount);
    }
}