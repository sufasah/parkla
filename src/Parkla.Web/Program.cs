using System.Reflection;
using System.Text.Json;
using Collector;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Parkla.Core.DTOs;
using Parkla.DataAccess.Contexts;
using Parkla.Web.Hubs;
using Parkla.Web.Options;
using Parkla.Web.SerialCom;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureServices(services => {
    services.AddOptions();
    services.Configure<WebOptions>(builder.Configuration.GetSection("Parkla"));

    services.AddControllers(o => {
        o.AllowEmptyInputInBodyModelBinding = true;
        o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    }).AddJsonOptions(options => {
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    }).AddFluentValidation(config => {
        config.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    });

    services.AddCors(o => {
        o.DefaultPolicyName = "allowAll";
        o.AddDefaultPolicy(b => {
            b.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(origin => true);
        });
    });

    services.AddGrpc();
    services.AddSignalR();

    services.AddHostedService<SerialReceiver>();

    services.AddDbContext<ParklaDbContext>(o => {
        o.UseNpgsql(builder.Configuration.GetConnectionString("parkla-admin"), b => {
            b.EnableRetryOnFailure(30);
            b.SetPostgresVersion(13,6);
        });
    });

    services.AddTransient<IValidator<ParkSpaceStatusDto>, ParkSpaceStatusValidator>();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();

app.UseCors("allow-all");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(builder => {
    builder.MapGrpcService<CollectorService>();
    builder.MapHub<CollectorHub>("/parkla");

    builder.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{id?}");
});

app.Run();
