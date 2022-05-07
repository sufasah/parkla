using FluentValidation;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Context;

namespace Parkla.Business.Concrete;
public class RealParkSpaceService : EntityServiceBase<RealParkSpace>
{
    public RealParkSpaceService(
        IEntityRepository<RealParkSpace> entityRepository, 
        IValidator<RealParkSpace> validator
    ) : base(entityRepository, validator)
    {
    }
}