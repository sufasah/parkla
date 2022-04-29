using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Library;
public abstract class HandlerBase
{
    private static readonly HashSet<HandlerBase> _handlerSet = new();
    public static T? GetInstance<T>() where T: HandlerBase, new()
    {
        var type = typeof(T);
        return (T?) GetInstance(type);
    }
    public static HandlerBase? GetInstance(Type type) {
        if(!type.IsClass || !type.IsPublic || type.IsAbstract || !type.IsSubclassOf(typeof(HandlerBase))) 
            return null;
        
        var setItem = _handlerSet.FirstOrDefault(x => x.GetType().Equals(type));
        if(setItem != null) return setItem;

        try {
            var newInstance = (HandlerBase?)Activator.CreateInstance(type, false);
            
            if(newInstance == null)
                return null;
                
            _handlerSet.Add(newInstance);
            return newInstance;
        }
        catch(Exception) {
            return null;
        }

    }
    // TO USE HANDLE METHOD IN APPLICATION, IN CHILD CLASS NOT OVERRIDE HANDLEASYNC BUT DEFINE HANDLE METHOD. IF HANDLEASYNC IS OVERRIDDEN, ONLY IT WILL BE WORKING.
    virtual public IEnumerable<ParkSpaceStatusDto> Handle(ReceiverType receiverType, object parameter) {
        return null;
    }

    // IF HANDLEASYNC OVERRIDDEN IT WILL BE CALLED OTHERWISE THIS BASE ONE WILL BE CALLED DURING APPLICATION. BY DEFAULT IT CALLS HANDLE METHOD.
    virtual public Task<IEnumerable<ParkSpaceStatusDto>> HandleAsync(ReceiverType receiverType, object parameter) {
        return Task.FromResult(Handle(receiverType, parameter));
    }
}