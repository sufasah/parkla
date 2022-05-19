using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Parkla.DataAccess.Extensions;
public static class EntityEntryExtensions
{
    public static EntityEntry<TEntity> CastGeneric<TEntity>(this EntityEntry entry) where TEntity : class
        => entry.Context.Entry((TEntity)entry.Entity);
}