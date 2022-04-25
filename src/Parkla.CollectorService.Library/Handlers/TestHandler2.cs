using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Library;

public class TestHandler2 : HandlerBase
{
    private TestHandler2(){}
    public override ParkSpaceStatusDto? Handle(ReceiverType receiverType, object param)
    {
        throw new NotImplementedException("THIS HANDLER WILL NOT BE CREATED BECAUSE OF PRIVATE CONSTRUCTOR");
    }
}