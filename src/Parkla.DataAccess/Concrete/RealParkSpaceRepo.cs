using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.Core.Exceptions;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class RealParkSpaceRepo<TContext> : EntityRepoBase<RealParkSpace, TContext>, IRealParkSpaceRepo 
    where TContext: DbContext, new()
{
    public override async Task DeleteAsync(RealParkSpace entity, CancellationToken cancellationToken = default)
    {
        using var context = new TContext();
        while (!cancellationToken.IsCancellationRequested)
        {
            var result = context.Attach(entity);
            await result.ReloadAsync(cancellationToken).ConfigureAwait(false);
            
            if(result.State == EntityState.Detached)
                throw new ParklaConcurrentDeletionException("The deleting realspace is already deleted by another user");
            
            try {
                result.State = EntityState.Deleted;
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                break;
            }
            catch(DbUpdateConcurrencyException) {
                context.ChangeTracker.Clear();
            }
        }
    }
}