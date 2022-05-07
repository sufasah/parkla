using FluentValidation;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Context;

namespace Parkla.Business.Concrete;
public class DistrictService : EntityServiceBase<District>
{
    public DistrictService(
        IEntityRepository<District> entityRepository, 
        IValidator<District> validator
    ) : base(entityRepository, validator)
    {
    }
}