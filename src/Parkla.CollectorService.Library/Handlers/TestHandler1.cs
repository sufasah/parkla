using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Library;

public class TestHandler1 : HandlerBase
{
    public override IEnumerable<ParkSpaceStatusDto> Handle(ReceiverType receiverType, object param)
    {
        throw new NotImplementedException("THIS WILL CAUSE AN EXCEPTION AND BECAUSE OF THAT ERROR WILL BE LOGGED");
    }
}