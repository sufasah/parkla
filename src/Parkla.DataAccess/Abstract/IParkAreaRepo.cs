using System.Linq.Expressions;
using Parkla.Core.Entities;

namespace Parkla.DataAccess.Abstract;

public interface IParkAreaRepo : IEntityRepository<ParkArea>
{
    new Task<Tuple<ParkArea, Park?>> AddAsync(
        ParkArea area, 
        CancellationToken cancellationToken = default
    );

    new Task<Tuple<ParkArea, Park?>> UpdateAsync(
        ParkArea area,
        Expression<Func<ParkArea, object?>>[] updateProps,
        bool updateOtherProps = true,
        CancellationToken cancellationToken = default
    );
    
    Task<ParkArea> UpdateTemplateAsync(
        ParkArea area,
        CancellationToken cancellationToken = default
    );

    new Task<Park?> DeleteAsync(
        ParkArea area, 
        CancellationToken cancellationToken = default
    );
}