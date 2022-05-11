using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class CityService : EntityServiceBase<City>, ICityService
{
    private readonly ICityRepo _cityRepo;
    private readonly IValidator<City> _validator;

    public CityService(
        ICityRepo cityRepo, 
        IValidator<City> validator
    ) : base(cityRepo, validator)
    {
        _cityRepo = cityRepo;
        _validator = validator;
    }

    public async Task<List<City>> SearchAsync(string search, CancellationToken cancellationToken = default)
    {
        return await _cityRepo.GetListAsync(x =>  x.Name!.ToLower().Contains(search.ToLower()), cancellationToken);
    }
}