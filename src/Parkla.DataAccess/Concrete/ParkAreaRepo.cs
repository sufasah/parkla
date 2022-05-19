using System.Linq.Expressions;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;
using Parkla.DataAccess.Extensions;

namespace Parkla.DataAccess.Concrete;

public class ParkAreaRepo<TContext> : EntityRepoBase<ParkArea, TContext>, IParkAreaRepo 
    where TContext: DbContext, new()
{
    private static readonly ParklaException _parkNotFound = new("Park of the adding area so also this area does not exist in database.", HttpStatusCode.BadRequest);

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
                .Where(x => x.ParkId == area.ParkId && x.Id != area.Id);
            
        (park.MinPrice, park.AvaragePrice, park.MaxPrice) = await FindMinAvgMaxAsync(
            query, 
            area.MinPrice, 
            area.AvaragePrice, 
            area.MaxPrice, 
            cancellationToken
        ).ConfigureAwait(false);
    }

    private static void SetXMin(EntityEntry entry, PropertyValues dbValues) 
    {
        var pxmin = entry.Property("xmin")!;
        pxmin.CurrentValue =  dbValues.GetValue<uint>("xmin");
        pxmin.OriginalValue =  dbValues.GetValue<uint>("xmin");
    }

    private static async Task RetryParkAndAreaMinAvgMax(TContext context, EntityEntry entry, ParkArea area, CancellationToken cancellationToken = default) {
        var ePark = entry.CastGeneric<Park>();
        var park = ePark.Entity;
        var dbval = await ePark.GetDatabaseValuesAsync(cancellationToken).ConfigureAwait(false);

        if(dbval == null)
            throw _parkNotFound;
        
        var dbPark = (Park)dbval.ToObject();
        SetXMin(ePark, dbval);

        if(
            ePark.Property(x => x.MinPrice).OriginalValue != dbPark.MinPrice || 
            ePark.Property(x => x.AvaragePrice).OriginalValue != dbPark.AvaragePrice || 
            ePark.Property(x => x.MaxPrice).OriginalValue != dbPark.MaxPrice
        ) {
            await FindParkMinAvgMaxAsync(context, area, park, cancellationToken).ConfigureAwait(false);        
        }
    }
    
    private static async Task<Park> InitParkAndAreaMinAvgMax(TContext context, ParkArea area, CancellationToken cancellationToken = default) {
        var park = await context.FindAsync<Park>(new object?[]{area.ParkId}, cancellationToken: cancellationToken).ConfigureAwait(false);

        if(park == null)
            throw _parkNotFound;
        
        var parkEntry = context.Entry(park);
        parkEntry.Property(x => x.MinPrice).IsModified = true;
        parkEntry.Property(x => x.AvaragePrice).IsModified = true;
        parkEntry.Property(x => x.MaxPrice).IsModified = true;
        
        (area.MinPrice, area.AvaragePrice, area.MaxPrice) = Pricing.FindMinAvgMax(area.Pricings);
        await FindParkMinAvgMaxAsync(context, area, park, cancellationToken).ConfigureAwait(false);

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
                await RetryParkAndAreaMinAvgMax(context, entry, area, cancellationToken).ConfigureAwait(false);
            }
            
            return true;
        }, cancellationToken).ConfigureAwait(false);

        if(cancelled) return new(area, null);
        return new(area, park);
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
        result.Property(x => x.MinPrice).IsModified = true;
        result.Property(x => x.AvaragePrice).IsModified = true;
        result.Property(x => x.MaxPrice).IsModified = true;

        var cancelled = await RetryOnConcurrencyErrorAsync(async () => 
        {
            var deletedPricings = context.Set<Pricing>()
                .Where(x => x.AreaId == area.Id && !area.Pricings!.Contains(x));
            context.RemoveRange(deletedPricings);

            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return false;
        }, 
        async (err) => 
        {
            var entry = err.Entries.Single();
            
            var addDeletedPricingAsync = async (Pricing pricing) => 
            {
                var pEntry = context.Entry(pricing);
                var dbPricing = await pEntry.GetDatabaseValuesAsync(cancellationToken);
                if(dbPricing == null) {
                    pricing.Id = null;
                    pEntry.State = EntityState.Added;
                }
            };

            if(entry.Entity is Pricing pri) 
            {
                var pricingdbval = await entry.GetDatabaseValuesAsync(cancellationToken).ConfigureAwait(false);
                if(pricingdbval == null)
                    await addDeletedPricingAsync(pri);
                else
                    SetXMin(entry, pricingdbval);

                return true;
            }
            else if(entry.Entity is Park) 
            {
                await RetryParkAndAreaMinAvgMax(context, entry, area).ConfigureAwait(false);
            }

            var dbval = await entry.GetDatabaseValuesAsync(cancellationToken).ConfigureAwait(false);
            if(dbval == null)
                throw new ParklaConcurrentDeletionException();
            
            foreach (var pricing in area.Pricings!)
                await addDeletedPricingAsync(pricing);

            SetXMin(entry, dbval);

            return true;
        }, cancellationToken).ConfigureAwait(false);

        if(cancelled) return new(area, null);
        return new(area, park);
    }


    public override async Task<Park?> DeleteAsync(ParkArea area, CancellationToken cancellationToken = default)
    {
        using var context = new TContext();
        var result = context.Remove(area);
        area.Pricings = Array.Empty<Pricing>();

        var park = await InitParkAndAreaMinAvgMax(context, area, cancellationToken).ConfigureAwait(false);

        var cancelled = await RetryOnConcurrencyErrorAsync(async () => {
            await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return false;
        },
        async (err) => {
            var entry = err.Entries.Single();

            if(entry.Entity is Park park) {
                var dbval = await entry.GetDatabaseValuesAsync(cancellationToken).ConfigureAwait(false);
                if(dbval == null)
                    entry.State = EntityState.Detached;
                
                await RetryParkAndAreaMinAvgMax(context, entry, area).ConfigureAwait(false);
                return true;
            }

            foreach (var ent in err.Entries)
            {
                var dbval = await ent.GetDatabaseValuesAsync(cancellationToken).ConfigureAwait(false);
                if(dbval == null)
                    entry.State = EntityState.Detached;
                else
                    SetXMin(ent, dbval);
            }

            return true;
        }, cancellationToken).ConfigureAwait(false);

        if(cancelled) return null;
        return park;
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