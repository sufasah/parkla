using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Collector;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Parkla.Core.Helpers;
using Parkla.Core.Options;
using Parkla.DataAccess.Contexts;
using Parkla.Web.Extensions;
using Parkla.Web.Helper;
using Parkla.Web.Hubs;
using Parkla.Web.SerialCom;


var builder = WebApplication.CreateBuilder(args);


builder.Configuration.AddConfiguration(new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("secret.json")
    .Build());

builder.WebHost.ConfigureServices(services => {
    ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en-US");
    
    services.AddOptions();
    services.Configure<WebOptions>(builder.Configuration.GetSection("Parkla"));
    services.Configure<SecretOptions>(builder.Configuration.GetSection("SecretParkla"));
    JwtHelper.SecretKey = Encoding.ASCII.GetBytes(builder.Configuration.GetValue<string>("SecretParkla:tokenSecret"));
    JwtHelper.TokenValidationParameters = new() {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(JwtHelper.SecretKey),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = new TimeSpan(0,0,0)
    };

    services.AddAuthentication(cfg => {
        cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, o => {
        o.RequireHttpsMetadata = false;
        o.SaveToken = true;
        o.TokenValidationParameters = JwtHelper.TokenValidationParameters;
        o.Events = new JwtBearerEvents() {
            OnAuthenticationFailed = context => {
                if(context.Exception.GetType() == typeof(SecurityTokenExpiredException)) {
                    context.HttpContext.Response.Headers.Add("Access-Control-Expose-Headers", "token-expired");
                    context.Response.Headers.Add("token-expired", "true");
                }
                return Task.CompletedTask;
            }
        };
    });

    services.AddAuthorization(o => {});


    services.AddControllers(o => {
        o.AllowEmptyInputInBodyModelBinding = true;
        o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
    }).AddJsonOptions(options => {
        options.JsonSerializerOptions.AllowTrailingCommas = true;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(null, false));
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

    services.AddSignalR(c => {

    }).AddJsonProtocol(c => {
        c.PayloadSerializerOptions.AllowTrailingCommas = true;
        c.PayloadSerializerOptions.PropertyNameCaseInsensitive = true;
        c.PayloadSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
        c.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        c.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter(null, false));
    });

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
}

app.UseAppExceptionHandler();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors("allowAll");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseEndpoints(builder => {
    builder.MapGrpcService<CollectorService>();
    builder.MapHub<ParklaHub>("/parkla");

    builder.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action}/{id?}");
});

app.Run();
