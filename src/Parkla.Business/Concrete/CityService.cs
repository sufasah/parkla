using FluentValidation;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Context;

namespace Parkla.Business.Concrete;
public class CityService : EntityServiceBase<City>
{
    public CityService(
        IEntityRepository<City> entityRepository, 
        IValidator<City> validator
    ) : base(entityRepository, validator)
    {
    }
}