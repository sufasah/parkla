using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class DistrictService : EntityServiceBase<District>, IDistrictService
{
    private readonly IDistrictRepo _repo;
    private readonly IValidator<District> _validator;

    public DistrictService(
        IDistrictRepo districtRepo, 
        IValidator<District> validator
    ) : base(districtRepo, validator)
    {
        _repo = districtRepo;
        _validator = validator;
    }
    public async Task<List<District>> SearchAsync(int cityId, string search, CancellationToken cancellationToken = default)
    {
        return await _repo.GetListAsync(x =>  x.Name!.ToLower().Contains(search.ToLower()) && x.CityId == cityId, cancellationToken);
    }
}