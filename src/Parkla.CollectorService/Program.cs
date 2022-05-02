using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Options;
using Parkla.CollectorService.Receivers;
using Parkla.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Collector;
using Parkla.CollectorService.OptionsManager;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureServices(services =>
{
    services.AddOptions();
    //services.Configure<CollectorOptions>((opt) => LoadCollectorOptions(opt, builder.Configuration));
    services.Configure<ParklaOptions>(builder.Configuration.GetSection("parkla"));

    services.AddGrpc();

    services.AddSingleton<HttpExporter>();
    services.AddSingleton<SerialExporter>();
    services.AddSingleton<GrpcExporter>();
    services.AddSingleton<SerialReceiver>();
    services.AddSingleton<HttpReceiver>();
    services.AddSingleton<GrpcReceiver>();
    
    services.AddSingleton<ParklaOptionsManager>();
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

var s = app.Services;
var parklaOptionsManager = s.GetService<ParklaOptionsManager>()!;
var logger = s.GetService<ILogger<Program>>()!;
var httpReceiver = s.GetService<HttpReceiver>()!;

var exporters = new ExporterBase[] {
    s.GetService<HttpExporter>()!,
    s.GetService<SerialExporter>()!,
    s.GetService<GrpcExporter>()!
};
var receivers = new ReceiverBase[] {
    httpReceiver,
    s.GetService<SerialReceiver>()!,
    s.GetService<GrpcReceiver>()!
};

parklaOptionsManager.Configure();

var startTask = Task.Run(() =>
{
    logger.LogInformation("START: Starting receiver and exporters");
    foreach (var exporter in exporters)
        exporter.Start();
    foreach (var receiver in receivers)
        receiver.Start();
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
    
    builder.MapGrpcService<CollectorService>();

    var httpPipes = HttpReceiverElem.HttpPipes;
    var endpointPipesMap = new Dictionary<string, List<Pipe>>();
    
    foreach (var pipe in httpPipes)
    {
        var receiver = (HttpReceiverElem)pipe.Receiver;
        var isGot = endpointPipesMap.TryGetValue(receiver.Endpoint, out var pipes);

        if(!isGot) {
            pipes = new();
            endpointPipesMap.Add(receiver.Endpoint, pipes);
        }

        pipes!.Add(pipe);
    }

    foreach(var endpointPipes in endpointPipesMap) {
        var endpoint = endpointPipes.Key;
        var pipes = endpointPipes.Value.ToArray();
        
        builder.MapPost(endpoint, async (HttpContext httpContext) => {
            await httpReceiver.ReceiveAsync(httpContext, pipes);
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