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
                Parkid = Guid.NewGuid(),
                Spaceid = "FIRST OBJECT OF TESTHANDLER [00000]",
                Status = ParkStatus.EMPTY,
                DateTime = DateTime.Now
            },
            new() {
                Parkid = Guid.NewGuid(),
                Spaceid = "SECOND OBJECT OF TESTHANDLER [00000]",
                Status = ParkStatus.EMPTY,
                DateTime = DateTime.Now
            },
        };
    }
}