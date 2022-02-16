using Microsoft.AspNetCore.Authorization;
using Parkla.Web.Requirements;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddAuthenticationCore(o => {
    o.DefaultAuthenticateScheme = "allowAll";
    o.DefaultScheme = "allowAll";
});

builder.Services.AddAuthorizationCore(o => {
    o.DefaultPolicy = new AuthorizationPolicyBuilder("allowAll")
        .AddRequirements(new IAuthorizationRequirement[]{
            new NoRequirement()
        })
        .Build();

});

builder.Services.AddControllersWithViews();

builder.Services.AddCors(o => {
    o.DefaultPolicyName = "allowAll";
    o.AddDefaultPolicy(b => {
        b.AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .SetIsOriginAllowed(origin => true);
    });
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

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.UseCors("allow-all");

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");;

app.Run();
