using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Library;

public class TestHandler5 : HandlerBase
{
    public override ParkSpaceStatusDto? Handle(ReceiverType receiverType, object param)
    {
        // This will return null and it will not exported
        return null;
    }
}