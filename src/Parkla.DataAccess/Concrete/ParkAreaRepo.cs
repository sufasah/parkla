using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class ParkAreaRepo<TContext> : EntityRepoBase<ParkArea, TContext>, IParkAreaRepo 
    where TContext: DbContext, new()
{
    public async Task<ParkArea> AddAsync(ParkArea area, CancellationToken cancellationToken = default) {
        using var context = new TContext();
        var result = context.Entry(area);
        result.State = EntityState.Added;       
        foreach (var pricing in area.Pricings!)
        {
            pricing.AreaId = area.Id;
            pricing.Area = area;
            var presult = context.Entry(pricing);
            presult.State = EntityState.Added;
        }
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        return area;
    }

    public async Task<ParkArea> UpdateAsync(
        ParkArea area,
        Expression<Func<ParkArea, object?>>[] updateProps,
        bool updateOtherProps = true,
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
        var newPricings = new List<Pricing>(area.Pricings!);
        var result = context.Attach(area);
    
        await result.Collection(x => x.Pricings!).LoadAsync(cancellationToken);
        result.State = updateOtherProps ? EntityState.Modified : EntityState.Unchanged;

        foreach (var prop in updateProps)
        {
            result.Property(prop).IsModified = !updateOtherProps;
        }

        area.Pricings  = newPricings;
        //result.Collection(x => x.Pricings).IsModified = true;
        await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
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