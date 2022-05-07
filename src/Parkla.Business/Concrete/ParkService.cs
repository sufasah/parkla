using FluentValidation;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ParkService : EntityServiceBase<Park>
{
    public ParkService(
        IEntityRepository<Park> entityRepository, 
        IValidator<Park> validator
    ) : base(entityRepository, validator)
    {
    }
}