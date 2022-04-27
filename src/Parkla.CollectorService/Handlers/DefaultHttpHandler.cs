using Parkla.Core.DTOs;
using Parkla.CollectorService.Library;

namespace Parkla.CollectorService.Handlers;
public class DefaultHttpHandler : HandlerBase
{
    // THIS HANDLE METHOD WILL BE CALLED WHEN A REQUEST IS SENT
    public override IEnumerable<ParkSpaceStatusDto> Handle(ReceiverType receiverType, object param)
    {
        if(receiverType != ReceiverType.HTTP)
            throw new ArgumentException("DefaultHttpHandler only handles http requests");

        var httpParam = (HttpReceiverParam) param;
        var httpContext = httpParam.HttpContext;
        var logger = httpParam.Logger;
        var request = httpContext.Request;

        ValueTask<ParkSpaceStatusDto?> valueTask;
        try {
            valueTask = request.ReadFromJsonAsync<ParkSpaceStatusDto?>();
        }
        catch (Exception e) {
            logger.LogInformation("DefaultHttpHandler: Request body could not be deserialized as json\n{}", e.Message);
            return Array.Empty<ParkSpaceStatusDto>();
        }
        var task = valueTask.AsTask();

        task.Wait();
        
        if(task.Result == null) return Array.Empty<ParkSpaceStatusDto>();
        return new ParkSpaceStatusDto[] {task.Result};
    }

}