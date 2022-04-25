using Parkla.Core.DTOs;

namespace Parkla.CollectorService.Library;
public abstract class HandlerBase
{
    private static readonly HashSet<HandlerBase> _handlerSet = new();
    public static T GetInstance<T>() where T: HandlerBase, new()
    {
        var type = typeof(T);
        return (T) GetInstance(type);
    }
    public static object GetInstance(Type type) {
        // ISSUBCLASSOF RETURN FALSE BUT IN PROGRAM.CS WITH NOT LOADED ASSEMBLY NEW MYHANDLER TYPE ISSUBCLASSOF HANDLERBASE TRUE
        // ALSO CASTING AT LINE 21 CAUSE ERROR. OTHER LOADED ASSEMBLY'S HANDLERBASE IS NOT SAME WITH APPDOMAIN ONE
        if(!type.IsClass || type.IsAbstract || !type.IsSubclassOf(typeof(HandlerBase))) 
            throw new ArgumentException("Given parameter must be a instantiable child class of HandlerBase with no argument public constructor");
        
        var setItem = _handlerSet.FirstOrDefault(x => x.GetType().Equals(type));
        if(setItem != null) return setItem;

        var newInstance = (HandlerBase)Activator.CreateInstance(type, false);
        _handlerSet.Add(newInstance);
        return newInstance;
    } 
    abstract public ParkSpaceStatusDto Handle(object paramsFacade);
}