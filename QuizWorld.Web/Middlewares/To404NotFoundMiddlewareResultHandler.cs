using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;

namespace QuizWorld.Web.Middlewares
{
    public class To404NotFoundMiddlewareResultHandler(AuthorizationMiddlewareResultHandler handler) : IAuthorizationMiddlewareResultHandler
    {
        private readonly string[] urlSegments = ["/logs", "/roles"];
        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = handler;

        public async Task HandleAsync(
            RequestDelegate next,
            HttpContext context,
            AuthorizationPolicy policy,
            PolicyAuthorizationResult authorizeResult)
        {
            if (RouteMatches(context) && (authorizeResult.Challenged || authorizeResult.Forbidden))
            {
                context.Response.StatusCode = 404;
                return;
            }

            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }

        private bool RouteMatches(HttpContext context)
        {
            var request = context.Request;
            foreach (var segment in urlSegments)
            {
                if (request.Path.StartsWithSegments(segment))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
