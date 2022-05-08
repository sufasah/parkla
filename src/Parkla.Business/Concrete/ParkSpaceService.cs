using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ParkSpaceService : EntityServiceBase<ParkSpace>, IParkSpaceService
{
    public ParkSpaceService(
        IParkSpaceRepo parkSpaceRepo, 
        IValidator<ParkSpace> validator
    ) : base(parkSpaceRepo, validator)
    {
    }
}