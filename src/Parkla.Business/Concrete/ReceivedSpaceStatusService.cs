using FluentValidation;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Context;

namespace Parkla.Business.Concrete;
public class ReceivedSpaceStatusService : EntityServiceBase<ReceivedSpaceStatus>
{
    public ReceivedSpaceStatusService(
        IEntityRepository<ReceivedSpaceStatus> entityRepository, 
        IValidator<ReceivedSpaceStatus> validator
    ) : base(entityRepository, validator)
    {
    }
}