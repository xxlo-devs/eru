using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace eru.PlatformClients.FacebookMessenger.Middleware
{
    public static class HttpResponseExtensions
    {
        public static async Task SendResponse(this HttpContext httpContext, HttpStatusCode responseCode)
        {
            httpContext.Response.StatusCode = (int)responseCode;
            await httpContext.Response.Body.WriteAsync(new byte[0]);
        }
        
        public static async Task SendOkResponse(this HttpContext httpContext, string body)
        {
            httpContext.Response.StatusCode = (int)HttpStatusCode.OK;
            await httpContext.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(body));
        }
    }
}