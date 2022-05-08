using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class DistrictService : EntityServiceBase<District>, IDistrictService
{
    public DistrictService(
        IDistrictRepo districtRepo, 
        IValidator<District> validator
    ) : base(districtRepo, validator)
    {
    }
}