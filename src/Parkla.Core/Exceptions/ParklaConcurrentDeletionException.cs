using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Parkla.Core.Exceptions
{
    public class ParklaConcurrentDeletionException : ParklaException
    {
        public ParklaConcurrentDeletionException(string? message, HttpStatusCode statusCode = HttpStatusCode.NotFound) : base(message, statusCode) {}

        public override async Task HandleException(HttpContext context) {
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)StatusCode!;
            var message = JsonSerializer.Serialize(Message ?? "The record has been deleted by another user.");
            await context.Response.WriteAsync(message);
        }
    }
}