using FluentValidation;
using Parkla.Business.Abstract;
using Parkla.Business.Bases;
using Parkla.Core.Entities;
using Parkla.DataAccess.Abstract;

namespace Parkla.Business.Concrete;
public class ReceivedSpaceStatusService : EntityServiceBase<ReceivedSpaceStatus>, IReceivedSpaceStatusService
{
    public ReceivedSpaceStatusService(
        IReceivedSpaceStatusRepo receivedSpaceStatusRepo, 
        IValidator<ReceivedSpaceStatus> validator
    ) : base(receivedSpaceStatusRepo, validator)
    {
    }
}