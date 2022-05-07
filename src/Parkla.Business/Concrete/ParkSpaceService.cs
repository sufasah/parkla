using FluentValidation;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ParkSpaceService : EntityServiceBase<ParkSpace>
{
    public ParkSpaceService(
        IEntityRepository<ParkSpace> entityRepository, 
        IValidator<ParkSpace> validator
    ) : base(entityRepository, validator)
    {
    }
}