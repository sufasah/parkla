using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;
using Parkla.DataAccess.Bases;

namespace Parkla.DataAccess.Concrete;

public class UserRepo<TContext> : EntityRepoBase<User, TContext>, IUserRepo 
    where TContext: DbContext, new()
{
    public override async Task<User?> GetAsync<Tkey>(
        Tkey id, 
        CancellationToken cancellationToken = default
    )   where Tkey: struct 
    {
        int iid = (int)(object)id;
        using var context = new TContext();
        User? result = await context.Set<User>()
            .AsNoTracking()
            .Include(x => x.City)
            .Include(x => x.District)
            .SingleOrDefaultAsync(x => x.Id == iid, cancellationToken)
            .ConfigureAwait(false);
            
        return result;
    }

    public override async Task<User> UpdateAsync(
        User user,
        Expression<Func<User, object?>>[] updateProps,
        bool updateOtherProps = true,
        CancellationToken cancellationToken = default
    ) {
        using var context = new TContext();
        User userClone = user;
        while(!cancellationToken.IsCancellationRequested) {
            var result = context.Entry(user);
            userClone = (User)result.CurrentValues.Clone().ToObject();
            result.State = updateOtherProps ? EntityState.Modified : EntityState.Unchanged;
            foreach (var prop in updateProps)
                result.Property(prop).IsModified = !updateOtherProps;

            try {
                await context.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                return await context.Set<User>()
                    .Include(x => x.City)
                    .Include(x => x.District)
                    .FirstAsync(x => x.Equals(user),cancellationToken)
                    .ConfigureAwait(false);
            } catch(DbUpdateConcurrencyException e) {
                var entry = e.Entries.Single();
                await entry.ReloadAsync(cancellationToken).ConfigureAwait(false);
                userClone.xmin = ((User)entry.Entity).xmin;
                context.ChangeTracker.Clear();
            }
        }
        return userClone;
    }
}