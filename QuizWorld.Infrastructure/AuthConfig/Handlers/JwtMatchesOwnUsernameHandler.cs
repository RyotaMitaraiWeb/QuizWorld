using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using QuizWorld.Common.Claims;
using QuizWorld.Infrastructure.AuthConfig.Requirements;

namespace QuizWorld.Infrastructure.AuthConfig.Handlers
{
    public class JwtMatchesOwnUsernameHandler(IHttpContextAccessor http) : AuthorizationHandler<JwtMatchesOwnUsernameRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, JwtMatchesOwnUsernameRequirement requirement)
        {
            string? username = GetUsername(context);
            string requestUsername = GetRequestUsername();

            if (username == requestUsername)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        private static string? GetUsername(AuthorizationHandlerContext context)
        {
            string? username = context.User.FindFirst(UserClaimsProperties.Username)?.Value.ToLower();
            return username;
        }

        private string GetRequestUsername()
        {
            return http.HttpContext?.Request.RouteValues["username"]?.ToString()?.ToLower() ?? string.Empty;
        }

        public const string Name = "MatchesOwnUsername";
    }
}
