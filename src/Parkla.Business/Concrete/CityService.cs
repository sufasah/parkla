using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class CityService : EntityServiceBase<City>, ICityService
{
    public CityService(
        ICityRepo cityRepo, 
        IValidator<City> validator
    ) : base(cityRepo, validator)
    {
    }
}