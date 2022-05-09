using System.Net;
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
            context.Response.StatusCode = (int)(StatusCode ?? HttpStatusCode.InternalServerError);
            await context.Response.WriteAsync(Message ?? "An unknown internal error occured. If this error occurs frequently, please inform us via information channels.");
        }
    }
}