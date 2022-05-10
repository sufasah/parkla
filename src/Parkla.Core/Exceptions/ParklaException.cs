using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Parkla.Core.Exceptions
{
    public class ParklaException : Exception
    {
        protected HttpStatusCode? StatusCode { get; }

        public ParklaException() {}

        public ParklaException(string? message, HttpStatusCode? statusCode) : base(message) {
            StatusCode = statusCode;
        }

        public virtual async Task HandleException(HttpContext context) {
            context.Response.ContentType = "application/json; charset=utf-8";
            context.Response.StatusCode = (int)(StatusCode ?? HttpStatusCode.InternalServerError);
            var message = JsonSerializer.Serialize(Message ?? "An unknown internal error occured. If this error occurs frequently, please inform us via information channels.");
            await context.Response.WriteAsync(message);
        }
    }
}