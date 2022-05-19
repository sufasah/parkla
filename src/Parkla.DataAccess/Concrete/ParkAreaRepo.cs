using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;
using Parkla.DataAccess.Extensions;

namespace Parkla.DataAccess.Concrete;

public class ParkAreaRepo<TContext> : EntityRepoBase<ParkArea, TContext>, IParkAreaRepo 
    where TContext: DbContext, new()
{
    private static async Task<Tuple<float?,float?,float?>> FindMinAvgMaxAsync(
        IQueryable<ParkArea> query, 
        float? externalMin = null, 
        float? externalAvarage = null, 
        float? externalMax = null, 
        CancellationToken cancellationToken = default
    ) {
        var mmaResult = await query.GroupBy(_ => 1)
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
        
        if(externalAvarage != null)
        newAvarage = count > 0 && newAvarage != null 
            ? newAvarage * (count / (count+1)) + externalAvarage / (count + 1)
            : externalAvarage;
                
        if(externalMin != null)
        newMin = newMin != null
            ? Math.Min((float)newMin, (float)externalMin)
            : externalMin;

        if(externalMax != null)
        newMax = newMax != null
            ? Math.Max((float)newMax, (float)externalMax)
            : externalMax;
        
        return new(newMin, newAvarage, newMax);
    }
    
    private static async Task FindParkMinAvgMaxAsync(TContext context, ParkArea area,  Park park, CancellationToken cancellationToken = default) {
        var query = context.Set<ParkArea>()
                .AsNoTracking()
                .Where(x => x.ParkId == area.ParkId);
            
        (park.MinPrice, park.AvaragePrice, park.MaxPrice) = await FindMinAvgMaxAsync(
            query, 
            area.MinPrice, 
            area.AvaragePrice, 
            area.MaxPrice, 
            cancellationToken
        ).ConfigureAwait(false);
    }

    public new async Task<Tuple<ParkArea, Park>> AddAsync(ParkArea area, CancellationToken cancellationToken = default) {
        using var context = new TContext();
        var result = context.Add(area);        
        var park = await context.FindAsync<Park>(new object?[]{area.ParkId}, cancellationToken: cancellationToken).ConfigureAwait(false);
        var parkNotFound = new ParklaException("Park of the adding area so also this area does not exist in database.", HttpStatusCode.BadRequest);

        if(park == null)
            throw parkNotFound;
        
        var parkEntry = context.Entry(park);
        parkEntry.Property(x => x.MinPrice).IsModified = true;
        parkEntry.Property(x => x.AvaragePrice).IsModified = true;
        parkEntry.Property(x => x.MaxPrice).IsModified = true;
        
        (area.MinPrice, area.AvaragePrice, area.MaxPrice) = Pricing.FindMinAvgMax(area.Pricings);
        await FindParkMinAvgMaxAsync(context, area, park, cancellationToken).ConfigureAwait(false);

        await RetryOnConcurrencyErrorAsync(async () => {
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return false;
        }, 
        async (err) => {
            var entry = err.Entries[0];
            var ePark = entry.CastGeneric<Park>();
            var dbval = await ePark.GetDatabaseValuesAsync(cancellationToken).ConfigureAwait(false);

            if(dbval == null)
                throw parkNotFound;
            
            var dbPark = (Park)dbval.ToObject();

            var pxmin = ePark.Property(x => x.xmin);
            park.xmin = dbPark.xmin;
            pxmin.OriginalValue = dbPark.xmin;

            if(
                ePark.Property(x => x.MinPrice).OriginalValue != dbPark.MinPrice || 
                ePark.Property(x => x.AvaragePrice).OriginalValue != dbPark.AvaragePrice || 
                ePark.Property(x => x.MaxPrice).OriginalValue != dbPark.MaxPrice
            ) {
                await FindParkMinAvgMaxAsync(context, area, park, cancellationToken).ConfigureAwait(false);        
            }

            return true;
        }, cancellationToken).ConfigureAwait(false);

        return new(area, park);
    }

    public override async Task<ParkArea> UpdateAsync(
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

        if(area.Pricings!.Any()) {
            (area.MinPrice!, area.AvaragePrice!, area.MaxPrice!) = Pricing.FindMinAvgMax(area.Pricings!)!;
        }
        
        await RetryOnConcurrencyErrorAsync(async () => {
            var deletedPricings = context.Set<Pricing>()
                .Where(x => x.AreaId == area.Id && !area.Pricings!.Contains(x));
            context.RemoveRange(deletedPricings);

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return false;
        }, 
        async (err) => {
            var entry = err.Entries.Single();
            
            var addDeletedPricingAsync = async (Pricing pricing) => {
                var pEntry = context.Entry(pricing);
                var dbPricing = await pEntry.GetDatabaseValuesAsync(cancellationToken);
                if(dbPricing == null) {
                    pricing.Id = null;
                    pEntry.State = EntityState.Added;
                }
            };

            if(entry.Entity is Pricing pri) {
                await addDeletedPricingAsync(pri);
                return true;
            }

            var aEntry = entry.CastGeneric<ParkArea>();
            var dbval = aEntry!.GetDatabaseValues();         
            if(dbval == null)
                throw new ParklaConcurrentDeletionException();
            
            foreach (var pricing in area.Pricings!)
                await addDeletedPricingAsync(pricing);

            var dbArea = dbval.ToObject() as ParkArea;
            var pxmin = aEntry.Property(x => x.xmin);
            area.xmin = dbArea!.xmin;
            pxmin.OriginalValue = dbArea!.xmin;

            return true;
        }, cancellationToken);

        return area;
    }

    public async Task<ParkArea> UpdateTemplateAsync(
        ParkArea area, 
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
        var newSpaces = area.Spaces != null ? new List<ParkSpace>(area.Spaces) : new();
        var entry = context.Attach(area);
        entry.State = EntityState.Unchanged;

        entry.Property(x => x.TemplateImage).IsModified = true;
        
        await entry.Collection(x => x.Spaces!).LoadAsync(cancellationToken);
        area.Spaces = newSpaces;

        await context.SaveChangesAsync(cancellationToken);
        return area;
    }
}