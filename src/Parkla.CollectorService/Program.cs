using Parkla.CollectorService.Enums;
using Parkla.CollectorService.Exporters;
using Parkla.CollectorService.Options;


var GetReceiver = (ConfigurationManager configuration, ReceiverType type, string receiverPath) => {
    Receiver result;

    if(type == ReceiverType.HTTP) {
        var httpReciever = configuration.GetRequiredSection($"{receiverPath}").Get<HttpReceiver>();
        httpReciever.Handler ??= "default";
        if(httpReciever.Endpoint == null) throw new ArgumentNullException("Endpoint","HttpReceiver Endpoint configuration value must be given"); 
        result = httpReciever;
    }
    else {
        var serialReceiver = configuration.GetSection($"{receiverPath}").Get<SerialReceiver>();
        serialReceiver.Handler ??= "default";
        if(serialReceiver.PortName == null) throw new ArgumentNullException("PortName","SerialReceiver PortName configuration value must be given");
        result = serialReceiver;
    }

    return result;
};

var GetExporter = (ConfigurationManager configuration, ExporterType type, string exporterPath) => {
    Exporter result;

    if(type == ExporterType.HTTP) {
        var httpExporter = configuration.GetSection($"{exporterPath}").Get<HttpExporter>();
        if(httpExporter.Url == null) throw new ArgumentNullException("Url","HttpExporter Url configuration value must be given");
        result = httpExporter;
    }
    else {
        var serialExporter = configuration.GetSection($"{exporterPath}").Get<SerialExporter>();
        if(serialExporter.PortName == null) throw new ArgumentNullException("Url","SerialExporter PortName configuration value must be given");
        result = serialExporter;
    }

    return result;
};

var GetPipeline = (ConfigurationManager configuration, string pipelinePath) => {
    List<Receiver> receivers = new();
    List<Exporter> exporters = new();
    
    var j = 0;
    while(true) {
        var receiverPath = $"{pipelinePath}:receivers:{j}";
        var type = configuration.GetValue<ReceiverType?>($"{receiverPath}:type");
        if(type == null) break;
        Receiver receiver = GetReceiver(configuration, (ReceiverType)type, receiverPath);
        receivers.Add(receiver);
        j++;
    }

    j = 0;
    while(true) {
        var exporterPath = $"{pipelinePath}:exporters:{j}";
        var type = configuration.GetValue<ExporterType?>($"{exporterPath}:type");
        if(type == null) break;
        Exporter exporter = GetExporter(configuration, (ExporterType)type, exporterPath);
        exporters.Add(exporter);
        j++;
    }

    if(!receivers.Any() && !exporters.Any())
        return null;
    
    return new Pipeline {
        Receivers = receivers.ToArray(),
        Exporters = exporters.ToArray()
    };
};

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureServices(services => {
    services.AddOptions();

    //services.AddHostedService<Worker>();

    services.Configure<CollectorOptions>(opt => {
        ConfigurationManager configuration = builder.Configuration;
        var pipelines = new List<Pipeline>();
        var i = 0;
        while(true) {
            var pipelinePath = $"parkla:pipelines:{i}";
            var pipeline = GetPipeline(configuration, pipelinePath);
            if(pipeline == null) break;
            pipelines.Add(pipeline);
            i++;
        }

        opt.Pipelines = pipelines.ToArray();
    });

    services.AddSingleton<HttpExportManager>();
    services.AddSingleton<SerialExportManager>();
    services.AddSingleton<ExportManager>();
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
        defaults: new {Controller = "HttpReceiver", action = "Receive"});
});


app.Run();