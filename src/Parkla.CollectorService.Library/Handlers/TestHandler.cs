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
                SpaceId = "FIRST OBJECT OF TESTHANDLER [00000]",
                Status = SpaceStatus.EMPTY,
                DateTime = DateTime.Now
            },
            new() {
                ParkId = Guid.NewGuid(),
                SpaceId = "SECOND OBJECT OF TESTHANDLER [00000]",
                Status = SpaceStatus.EMPTY,
                DateTime = DateTime.Now
            },
        };
    }
}