using Parkla.Core.DTOs;
using Parkla.Core.Enums;

namespace Parkla.CollectorService.Library;

public class TestHandler : HandlerBase
{
    public override IEnumerable<ParkSpaceStatusDto> Handle(ReceiverType receiverType, object param)
    {
        // THIS HANDLER WILL WORK AND EXPORT BELOW
        return new ParkSpaceStatusDto[]{
            new() {
                ParkId = Guid.NewGuid(),
                SpaceId = 1111111111,
                Status = SpaceStatus.EMPTY,
                DateTime = DateTime.UtcNow
            },
            new() {
                ParkId = Guid.NewGuid(),
                SpaceId = 222222222,
                Status = SpaceStatus.OCCUPIED,
                DateTime = DateTime.UtcNow
            },
        };
    }
}