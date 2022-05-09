using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Parkla.Core.Exceptions;

namespace Parkla.Web.Extensions;
public static class AppExceptionHandler
{
    public static void UseAppExceptionHandler(this WebApplication app) {
        var logger = app.Services.GetService<ILogger<WebApplication>>()!;
        app.UseExceptionHandler(o => {
            o.Run(async context => {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerPathFeature>()!;
                var error = exceptionHandlerFeature.Error;

                if(error is ParklaException) {
                    await (error as ParklaException)!.HandleException(context);
                }
                else {
                    var task = context.Response.WriteAsync("An unknown internal error occured. If this error occurs frequently, please inform us via information channels.").ConfigureAwait(false);
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    logger.LogError(error, "AppExceptionHandler: An exception handling");
                    await task;
                }
            });
        });
    }
}