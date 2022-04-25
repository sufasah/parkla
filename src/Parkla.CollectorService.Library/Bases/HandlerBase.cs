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
    public static object? GetInstance(Type type) {
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
    abstract public ParkSpaceStatusDto? Handle(ReceiverType receiverType, object parameter);
}