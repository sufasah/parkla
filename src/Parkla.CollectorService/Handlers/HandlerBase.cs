using System.Reflection;
using Parkla.CollectorService.DTOs;

namespace Parkla.CollectorService.Handlers;
public abstract class HandlerBase
{
    private static readonly HashSet<HandlerBase> _handlerSet = new();
    public static T GetInstance<T>() where T: HandlerBase, new()
    {
        var type = typeof(T);
        var setItem = _handlerSet.FirstOrDefault(x => x.GetType().Equals(type));
        if(setItem != null) return (T)setItem;

        var newInstance = Activator.CreateInstance<T>();
        _handlerSet.Add(newInstance);
        return newInstance;
    }
    abstract public ParkSpaceStatusDto Handle(object paramsFacade);
}