using Collector;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Parkla.Core.DTOs;
using Parkla.Web.Requirements;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureServices(services => {
    services.AddAuthenticationCore(o => {
        o.DefaultAuthenticateScheme = "allowAll";
        o.DefaultScheme = "allowAll";
    });

    services.AddAuthorizationCore(o => {
        o.DefaultPolicy = new AuthorizationPolicyBuilder("allowAll")
            .AddRequirements(new IAuthorizationRequirement[]{
                new NoRequirement()
            })
            .Build();

    });

    services.AddControllers().AddFluentValidation(config => {

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

    builder.MapControllerRoute(
        name: "default",
        pattern: "{controller}/{action=Index}/{id?}");
});

app.Run();
