using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Parkla.Core.Exceptions;

namespace Parkla.Web.Extensions;
public static class AppExceptionHandler
{
    public static async Task WriteMessage(
        HttpContext context, 
        string message, 
        HttpStatusCode statusCode,
        ILogger? logger = null,
        Exception? error = null

    ) {
        context.Response.StatusCode = (int)statusCode;
        logger?.LogError(error, "AppExceptionHandler: An exception caught");
        await context.Response.WriteAsync(JsonSerializer.Serialize(message)).ConfigureAwait(false);
    }
    public static void UseAppExceptionHandler(this WebApplication app) {
        var logger = app.Services.GetService<ILogger<WebApplication>>()!;
        app.UseExceptionHandler(o => {
            o.Run(async context => {
                var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerPathFeature>()!;
                var error = exceptionHandlerFeature.Error;
                context.Response.ContentType = "application/json; charset=utf-8";

                if(error is ParklaException) {
                    await (error as ParklaException)!.HandleException(context);
                }
                else if(error is DbUpdateConcurrencyException) {
                    await WriteMessage(
                        context,
                        "Concurrent database record update detected.",
                        HttpStatusCode.Conflict,
                        logger,
                        error
                    ).ConfigureAwait(false);
                }
                else if(error is DbUpdateException) {
                    await WriteMessage(
                        context,
                        "Error has been caught while updating the database data. Most probably the data inside request is not consistent.",
                        HttpStatusCode.BadRequest,
                        logger,
                        error
                    ).ConfigureAwait(false);
                }
                else {
                    await WriteMessage(
                        context,
                        "An undefined internal error occured. If this error occurs frequently, please inform us via information channels.",
                        HttpStatusCode.InternalServerError,
                        logger,
                        error
                    ).ConfigureAwait(false);
                }
            });
        });
    }
}