using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Library;

public class TestHandler6 : HandlerBase
{
    public override async Task<IEnumerable<ParkSpaceStatusDto>> HandleAsync(ReceiverType receiverType, object parameter)
    {
        return null;
    }
}