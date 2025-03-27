using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EtaxService.Installers
{
    public class HeadersAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            var path = request.Path.Value;

            if (path != null && (
                path.StartsWith("/api/auth/test-login") ||
                path.StartsWith("/api/auth/login") ||
                path.StartsWith("/swagger") ||
                path.StartsWith("/health")))
            {
                base.OnActionExecuting(context);
                return;
            }

            if (request.Method != "GET" &&
                (!request.Headers.ContainsKey("Content-Type") ||
                 !request.Headers["Content-Type"].ToString().Contains("application/json")))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status415UnsupportedMediaType,
                    Content = "Content-Type must be application/json"
                };
                return;
            }

            var authHeader = request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Content = "Authorization header is required"
                };
                return;
            }

            if (!authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = new ContentResult
                {
                    StatusCode = StatusCodes.Status401Unauthorized,
                    Content = "Invalid Authorization header format"
                };
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}
