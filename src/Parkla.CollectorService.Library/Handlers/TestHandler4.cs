using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Library;

public abstract class TestHandler4 : HandlerBase
{
    public override ParkSpaceStatusDto? Handle(ReceiverType receiverType, object param)
    {
        throw new NotImplementedException("THIS HANDLER WILL NOT BE INSTANTIATED BECAUSE OF IT IS ABSTRACT");
    }
}