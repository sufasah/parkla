using System.Reflection;
using System.Text;
using System.Text.Json;
using Collector;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Parkla.Core.Helpers;
using Parkla.Core.Options;
using Parkla.DataAccess.Contexts;
using Parkla.Web.Extensions;
using Parkla.Web.Helper;
using Parkla.Web.Hubs;
using Parkla.Web.SerialCom;


var builder = WebApplication.CreateBuilder(args);
var deneme = BCrypt.Net.BCrypt.HashPassword("examplepass", BCrypt.Net.BCrypt.GenerateSalt(11)+"examplepepper");
var verified = BCrypt.Net.BCrypt.Verify("examplepass",deneme);
builder.Configuration.AddConfiguration(new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("secret.json")
    .Build());

builder.WebHost.ConfigureServices(services => {
    services.AddOptions();
    services.Configure<WebOptions>(builder.Configuration.GetSection("Parkla"));
    services.Configure<SecretOptions>(builder.Configuration.GetSection("SecretParkla"));
    JwtHelper.SecretKey = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("SecretParkla:tokenSecret"));

    services.AddAuthentication(cfg => {
        cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o => {
        o.RequireHttpsMetadata = false;
        o.SaveToken = true;
        o.TokenValidationParameters = JwtHelper.TokenValidationParameters;
    });

    services.AddAuthorization(o => {
    });


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

    ServiceCollectionHelper.AddDependencies<ParklaDbContext>(services);

    services.AddAutoMapper(cfg => {
        
    }, Assembly.GetExecutingAssembly());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseDeveloperExceptionPage();
}
else {
    app.UseAppExceptionHandler();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

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
