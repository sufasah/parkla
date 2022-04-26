using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Library;

public class TestHandler3
{
    public IEnumerable<ParkSpaceStatusDto> Handle(ReceiverType receiverType, object param)
    {
        throw new NotImplementedException("THIS HANDLER WILL NOT BE INSTANTIATED BECAUSE OF IT DOES NOT INHERIT HANDLERBASE");
    }
}