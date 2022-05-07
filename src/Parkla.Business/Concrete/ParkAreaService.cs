using FluentValidation;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ParkAreaService : EntityServiceBase<ParkArea>
{
    public ParkAreaService(
        IEntityRepository<ParkArea> entityRepository, 
        IValidator<ParkArea> validator
    ) : base(entityRepository, validator)
    {
    }
}