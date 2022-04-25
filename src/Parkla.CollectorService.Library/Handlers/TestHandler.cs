using Parkla.Core.DTOs;
using Parkla.Core.Enums;

namespace Parkla.CollectorService.Library;

public class TestHandler : HandlerBase
{
    public override ParkSpaceStatusDto? Handle(ReceiverType receiverType, object param)
    {
        // THIS HANDLER WILL WORK AND EXPORT BELOW
        return new(){
            Parkid = Guid.NewGuid(),
            Spaceid = "Manually instantiated",
            Status = ParkStatus.EMPTY
        };
    }
}