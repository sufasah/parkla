using Parkla.Core.DTOs;
using Parkla.Core.Enums;

namespace Parkla.CollectorService.Library;

public class TestHandler1 : HandlerBase
{
    public override async Task<IEnumerable<ParkSpaceStatusDto>> HandleAsync(ReceiverType receiverType, object parameter)
    {
        return await Task.Run(() => {
            return new ParkSpaceStatusDto[] {
                    new() {
                    ParkId = Guid.NewGuid(),
                    SpaceId = 33333,
                    Status = SpaceStatus.OCCUPIED,
                    DateTime = DateTime.UtcNow
                },
            };
        });
    }
}