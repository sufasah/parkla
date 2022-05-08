using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ParkService : EntityServiceBase<Park>, IParkService
{
    public ParkService(
        IParkRepo parkRepo, 
        IValidator<Park> validator
    ) : base(parkRepo, validator)
    {
    }
}