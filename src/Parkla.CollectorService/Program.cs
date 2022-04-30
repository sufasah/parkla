using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Options;
using Parkla.CollectorService.Library;
using System.Reflection;
using Parkla.CollectorService.Handlers;
using Parkla.CollectorService.Receivers;
using Parkla.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Parkla.CollectorService.Generators;

var LoadCollectorOptions = (CollectorOptions opt, ConfigurationManager configuration) => {
    var GetHandler = (string handlerStr, Type defaultHandler) => {
        if (string.IsNullOrWhiteSpace(handlerStr) || handlerStr == "default")
            return HandlerBase.GetInstance(defaultHandler)!;

        Type? handlerType = Assembly.GetExecutingAssembly()
            .GetType(handlerStr);
        
        if(handlerType == null) {
            if (opt.pluginAssembly == null)
                throw new DllNotFoundException($"Plugin library could not found while finding handler '{handlerStr}'");

            handlerType = opt.pluginAssembly.ExportedTypes
                .FirstOrDefault(x => x.Name == handlerStr);

            if (handlerType == null)
                throw new TypeUnloadedException($"Handler type '{handlerStr}' could not found in loaded plugin library");
        }

        var handler = HandlerBase.GetInstance(handlerType);
        
        if(handler == null)
            throw new InvalidCastException($"Handler with '{handlerStr}' value is not a correctly built handler class. Check this handler class is a valid HandlerBase child");
        
        return handler;
    };
    
    var GetHttpReceiver = (IConfigurationSection receiver) => {
        var httpReciever = receiver.Get<HttpReceiverOptions>();
        var handlerStr = receiver.GetSection("handler").Value;
        httpReciever.Handler = GetHandler(handlerStr, typeof(DefaultHttpHandler));

        if (string.IsNullOrWhiteSpace(httpReciever.Endpoint))
            throw new ArgumentNullException("Endpoint", "HttpReceiver Endpoint configuration value must be a valid http url");
            
        return httpReciever;
    };

    var GetSerialReceiver = (IConfigurationSection receiver) => {
        var serialReceiver = receiver.Get<SerialReceiverOptions>();
        var handlerStr = receiver.GetSection("handler").Value;
        serialReceiver.Handler = GetHandler(handlerStr, typeof(DefaultSerialHandler));

        if (string.IsNullOrWhiteSpace(serialReceiver.PortName))
            throw new ArgumentNullException("PortName", "SerialReceiver PortName configuration value must be given");

        return serialReceiver;
    };

    var GetHttpExporter = (IConfigurationSection exporter) => {
        var httpExporter = exporter.Get<HttpExporterOptions>();
        if(string.IsNullOrWhiteSpace(httpExporter.Url.OriginalString))
            throw new ArgumentNullException("Url", "HttpExporter Url value must be a valid http url");
        return httpExporter;
    };

    var GetSerialExporter = (IConfigurationSection exporter) => {
        var serialExporter = exporter.Get<SerialExporterOptions>();
        if (string.IsNullOrWhiteSpace(serialExporter.PortName))
            throw new ArgumentNullException("Url", "SerialExporter PortName value must be given");
        return serialExporter;
    };

    var GetPipeline = (IConfigurationSection pipeline) => {
        var httpReceivers = new List<HttpReceiverOptions>();
        var serialReceivers = new List<SerialReceiverOptions>();
        var httpExporters = new List<HttpExporterOptions>();
        var serialExporters = new List<SerialExporterOptions>();

        var receivers = pipeline.GetSection("receivers")
            .GetChildren()
            .ToArray();
        
        var exporters = pipeline.GetSection("exporters")
            .GetChildren()
            .ToArray();

        foreach(var receiver in receivers) {
            var type = receiver.GetSection("type").Get<ReceiverType?>();
            if(type == null) continue;

            if (type == ReceiverType.HTTP)
                httpReceivers.Add(GetHttpReceiver(receiver));
            else if (type == ReceiverType.SERIAL)
                serialReceivers.Add(GetSerialReceiver(receiver));
        }

        foreach(var exporter in exporters) {
            var type = exporter.GetSection("type").Get<ExporterType?>();
            if (type == null) continue;

            if (type == ExporterType.HTTP)
                httpExporters.Add(GetHttpExporter(exporter));
            else if (type == ExporterType.SERIAL)
                serialExporters.Add(GetSerialExporter(exporter));
        }

        if (!httpReceivers.Any() && !serialReceivers.Any() && !httpExporters.Any() && !serialExporters.Any())
            return null;

        return new PipelineOptions() {
            HttpReceivers = httpReceivers.ToArray(),
            SerialReceivers = serialReceivers.ToArray(),
            HttpExporters = httpExporters.ToArray(),
            SerialExporters = serialExporters.ToArray()
        };
    };

    // -------------------------- LOAD PLUGIN LIBRARY
    var pluginLibrary = configuration.GetValue<string>("parkla:pluginLibrary");
    if (!string.IsNullOrWhiteSpace(pluginLibrary)) {
        var dllFile = new FileInfo(
            pluginLibrary.Contains('/') || pluginLibrary.Contains('\\')
            ? pluginLibrary
            : Path.Combine(".", pluginLibrary)
        );
        opt.pluginAssembly = Assembly.LoadFrom(dllFile.FullName);
    }

    // -------------------------- LOAD PIPELINES
    var pipelines = new List<PipelineOptions>();
    var children = configuration.GetSection("parkla:pipelines").GetChildren();

    foreach(var child in children)
        pipelines.Add(GetPipeline(child));

    opt.Pipelines = pipelines.ToArray();

};


var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureServices(services =>
{
    services.AddOptions();

    services.Configure<CollectorOptions>((opt) => LoadCollectorOptions(opt, builder.Configuration));

    services.AddSingleton<HttpExporter>();
    services.AddSingleton<SerialExporter>();
    services.AddSingleton<SerialReceiver>();
    services.AddSingleton<HttpReceiver>();
    services.AddSingleton<SerialPortPool>();
});

builder.WebHost.ConfigureServices(services =>
{
    services.AddCors(o =>
    {
        o.DefaultPolicyName = "allowAll";
        o.AddDefaultPolicy(b =>
        {
            b.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(origin => true);
        });
    });
    services.AddHttpClient();
});

var app = builder.Build();


var httpReceiver = app.Services.GetService<HttpReceiver>()!;
var serialReceiver = app.Services.GetService<SerialReceiver>()!;
var httpExporter = app.Services.GetService<HttpExporter>()!;
var serialExporter = app.Services.GetService<SerialExporter>()!;
var logger = app.Services.GetService<ILogger<Program>>()!;

var startTask = Task.Run(() =>
{
    logger.LogInformation("START: Starting receiver and exporters");
    serialExporter.Start();
    serialReceiver.Start();
    httpReceiver.Start();
});

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseDeveloperExceptionPage();
}

app.UseRouting();

app.UseCors("allow-all");

app.UseEndpoints(builder =>
{
    /*builder.MapControllerRoute(
        "HttpReceiver",
        "{**all}",
        defaults: new { Controller = "HttpReceiver", action = "CatchAllRequests" });*/
    
    startTask.Wait();
    var httpPipelinesList = httpReceiver.GetHttpPipelinesList();
    
    foreach (var httpPipelines in httpPipelinesList)
    {
        builder.MapPost(httpPipelines.Endpoint, async (HttpContext httpContext) => {
            await httpReceiver.ReceiveAsync(httpContext, httpPipelines);
        });
    }

    builder.Map("/ECHO/TO/LOG",([FromBody] IEnumerable<ParkSpaceStatusDto> dtos) => {
        foreach (var dto in dtos)
        {
            logger.LogWarning(@"
                '/ECHO/ENDPOINT' RECEIVED THE DATA:
                Parkid='{}' Spaceid='{}' Status='{}' DateTime='{}'
            ",dto.Parkid, dto.Spaceid, dto.Status, dto.DateTime);
        }
    });
});

logger.LogInformation("START: Receiver and exporters are started");
app.Run();