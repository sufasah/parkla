using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Parkla.Core.Exceptions
{
    public class ParklaConcurrentDeletionException : ParklaException
    {
        protected HttpStatusCode? StatusCode { get; }

        public ParklaConcurrentDeletionException() {}

        public override async Task HandleException(HttpContext context) {
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)(StatusCode ?? HttpStatusCode.InternalServerError);
            var message = JsonSerializer.Serialize("The record has been deleted by another user.");
            await context.Response.WriteAsync(message);
        }
    }
}