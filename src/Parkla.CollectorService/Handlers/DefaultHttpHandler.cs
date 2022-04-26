using Parkla.Core.DTOs;
using Parkla.CollectorService.Library;

namespace Parkla.CollectorService.Handlers;
public class DefaultHttpHandler : HandlerBase
{
    // THIS HANDLE METHOD WILL BE CALLED WHEN A REQUEST IS SENT
    public override ParkSpaceStatusDto Handle(ReceiverType receiverType, object param)
    {
        if(receiverType != ReceiverType.HTTP)
            throw new ArgumentException("DefaultHttpHandler only handles http requests");

        var httpParam = (HttpReceiverParam) param;
        var httpContext = httpParam.httpContext;
        var request = httpContext.Request;

        var valueTask = request.ReadFromJsonAsync<ParkSpaceStatusDto>();
        var task = valueTask.AsTask();

        task.Wait();
        return task.Result;
    }

}