using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Options;
using Parkla.CollectorService.Library;
using System.Reflection;
using Parkla.CollectorService.Handlers;
using Parkla.CollectorService;
using Parkla.CollectorService.Receivers;

Assembly pluginDll = null;

var GetHandler = (string handlerStr, Type defaultHandler) => {
    if(handlerStr == "default")
        return defaultHandler;

    if(pluginDll == null)
        throw new DllNotFoundException($"Plugin library could not found while finding handler '{handlerStr}'");
    
    Type? handlerType = pluginDll.ExportedTypes.FirstOrDefault(x => x.Name == handlerStr);

    if(handlerType == null)
        throw new TypeUnloadedException($"Handler type '{handlerStr}' could not found in loaded plugin library");

    return handlerType;
};

var GetHttpReceiver = (ConfigurationManager configuration, string receiverPath) => {
    var httpReciever = configuration.GetRequiredSection($"{receiverPath}").Get<HttpReceiverOptions>();
    var handlerStr = configuration.GetValue<string>($"{receiverPath}:handler") ?? "default";

    var handlerType = GetHandler(handlerStr,typeof(DefaultHttpHandler));
    
    httpReciever.Handler = (HandlerBase?)HandlerBase.GetInstance(handlerType);
    if(httpReciever.Handler == null)
        httpReciever.Handler = HandlerBase.GetInstance<DefaultSerialHandler>()!;
    
    if(httpReciever.Endpoint == null) 
        throw new ArgumentNullException("Endpoint","HttpReceiver Endpoint configuration value must be given"); 
    
    return httpReciever;
};

var GetSerialReceiver = (ConfigurationManager configuration, string receiverPath) => {
    var serialReceiver = configuration.GetSection($"{receiverPath}").Get<SerialReceiverOptions>();
    var handlerStr = configuration.GetValue<string>($"{receiverPath}:handler") ?? "default";
        
    var handlerType = GetHandler(handlerStr, typeof(DefaultSerialHandler));
    
    serialReceiver.Handler = (HandlerBase?)HandlerBase.GetInstance(handlerType);
    if(serialReceiver.Handler == null)
        serialReceiver.Handler = HandlerBase.GetInstance<DefaultSerialHandler>()!;

    if(serialReceiver.PortName == null) 
        throw new ArgumentNullException("PortName","SerialReceiver PortName configuration value must be given");
    
    return serialReceiver;
};

var GetHttpExporter = (ConfigurationManager configuration, string exporterPath) => {
    var httpExporter = configuration.GetSection($"{exporterPath}").Get<HttpExporterOptions>();
    if(httpExporter.Url == null) throw new ArgumentNullException("Url","HttpExporter Url configuration value must be given");
    return httpExporter;
};

var GetSerialExporter = (ConfigurationManager configuration, string exporterPath) => {
    var serialExporter = configuration.GetSection($"{exporterPath}").Get<SerialExporterOptions>();
    if(serialExporter.PortName == null) throw new ArgumentNullException("Url","SerialExporter PortName configuration value must be given");
    return serialExporter;
};

var GetPipeline = (ConfigurationManager configuration, string pipelinePath) => {
    List<HttpReceiverOptions> httpReceivers = new();
    List<SerialReceiverOptions> serialReceivers = new();
    List<HttpExporterOptions> httpExporters = new();
    List<SerialExporterOptions> serialExporters = new();
    
    var j = 0;
    while(true) {
        var receiverPath = $"{pipelinePath}:receivers:{j}";
        var type = configuration.GetValue<ReceiverType?>($"{receiverPath}:type");
        if(type == null) break;

        if(type == ReceiverType.HTTP)
            httpReceivers.Add(GetHttpReceiver(configuration, receiverPath));
        else if(type == ReceiverType.SERIAL)
            serialReceivers.Add(GetSerialReceiver(configuration, receiverPath));
        j++;
    }

    j = 0;
    while(true) {
        var exporterPath = $"{pipelinePath}:exporters:{j}";
        var type = configuration.GetValue<ExporterType?>($"{exporterPath}:type");
        if(type == null) break;

        if(type == ExporterType.HTTP)
            httpExporters.Add(GetHttpExporter(configuration, exporterPath));
        else if(type == ExporterType.SERIAL)
            serialExporters.Add(GetSerialExporter(configuration, exporterPath));
        j++;
    }

    if(!httpReceivers.Any() && !serialReceivers.Any() && !httpExporters.Any() && !serialExporters.Any())
        return null;
    
    return new PipelineOptions {
        HttpReceivers = httpReceivers.ToArray(),
        SerialReceivers = serialReceivers.ToArray(),
        HttpExporters = httpExporters.ToArray(),
        SerialExporters = serialExporters.ToArray()
    };
};

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureServices(services => {
    services.AddOptions();

    services.AddHostedService<Worker>();

    services.Configure<CollectorOptions>(opt => {
        ConfigurationManager configuration = builder.Configuration;
        
        var pluginLibrary = configuration.GetValue<string>("parkla:pluginLibrary");
        if(pluginDll == null && pluginLibrary != null) {
            var dllFile = new FileInfo(
                pluginLibrary.Contains('/') || pluginLibrary.Contains('\\') 
                ? pluginLibrary 
                : Path.Combine(".",pluginLibrary)
            );
            pluginDll = Assembly.LoadFrom(dllFile.FullName);
        }

        var pipelines = new List<PipelineOptions>();
        var i = 0;
        while(true) {
            var pipelinePath = $"parkla:pipelines:{i}";
            var pipeline = GetPipeline(configuration, pipelinePath);
            if(pipeline == null) break;
            pipelines.Add(pipeline);
            i++;
        }

        opt.pluginAssembly = pluginDll;
        opt.Pipelines = pipelines.ToArray();
    });

    services.AddSingleton<HttpExportManager>();
    services.AddSingleton<SerialExportManager>();
    services.AddSingleton<ExportManager>();
    services.AddSingleton<SerialReceiver>();
    services.AddSingleton<HttpReceiver>();
});

builder.WebHost.ConfigureServices(services => {
    services.AddCors(o => {
        o.DefaultPolicyName = "allowAll";
        o.AddDefaultPolicy(b => {
            b.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(origin => true);
        });
    });

    services.AddAuthentication();
    services.AddAuthorization();

    services.AddHttpClient();

    services.AddControllers();
});

var app = builder.Build();


if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseCors("allow-all");

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(builder => {
    builder.MapControllerRoute(
        "HttpReceiver",
        "{**all}",
        defaults: new {Controller = "HttpReceiver", action = "CatchAllRequests"});
});


app.Run();