using System.Globalization;
using System.Reflection;
using Parkla.CollectorService.Library;
using Parkla.CollectorService.Options;

namespace Parkla.CollectorService.OptionsManager;
public abstract class ReceiverElemBase
{
    public HandlerBase Handler { get; set; }
    public static ReceiverElemBase GetReceiverElem(ReceiverType receiverType) {
        var prefix = receiverType.ToString();
        prefix = char.ToUpper(prefix[0], CultureInfo.InvariantCulture) + prefix[1..].ToLower(CultureInfo.InvariantCulture);
        var typeStr = $"{typeof(ReceiverElemBase).Namespace}.{prefix}ReceiverElem";
        var type = Type.GetType(typeStr);
        
        if(type == null)
            throw new TypeUnloadedException(typeStr);
        
        var instance = Activator.CreateInstance(type)!;
        return (ReceiverElemBase)instance;
    }

    protected HandlerBase GetHandler(string? handlerStr, Type defaultHandler, Assembly pluginAssembly) {
        if (string.IsNullOrWhiteSpace(handlerStr) || handlerStr == "default")
            return HandlerBase.GetInstance(defaultHandler)!;

        Type? handlerType = Assembly.GetExecutingAssembly()
            .GetType(handlerStr);

        if(handlerType == null) {
            if (pluginAssembly == null)
                throw new DllNotFoundException($"Plugin library could not found while finding handler '{handlerStr}'");

            handlerType = pluginAssembly.ExportedTypes
                .FirstOrDefault(x => x.Name == handlerStr);

            if (handlerType == null)
                throw new TypeUnloadedException($"Handler type '{handlerStr}' could not found in loaded plugin library");
        }
        
        var handler = HandlerBase.GetInstance(handlerType);

        if(handler == null)
            throw new InvalidCastException($"Handler with '{handlerStr}' value is not a correctly built handler class. Check this handler class is a valid HandlerBase child");

        return handler;
    }

    public abstract void ConfigureReceiver(ReceiverOptions receiver, ExporterElemBase[] exporters, Assembly pluginAssembly);
}