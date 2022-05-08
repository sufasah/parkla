using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class RealParkSpaceService : EntityServiceBase<RealParkSpace>, IRealParkSpaceService
{
    public RealParkSpaceService(
        IRealParkSpaceRepo realParkSpaceRepo, 
        IValidator<RealParkSpace> validator
    ) : base(realParkSpaceRepo, validator)
    {
    }
}