using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ParkAreaService : EntityServiceBase<ParkArea>, IParkAreaService
{
    public ParkAreaService(
        IParkAreaRepo parkAreaRepo, 
        IValidator<ParkArea> validator
    ) : base(parkAreaRepo, validator)
    {
    }
}