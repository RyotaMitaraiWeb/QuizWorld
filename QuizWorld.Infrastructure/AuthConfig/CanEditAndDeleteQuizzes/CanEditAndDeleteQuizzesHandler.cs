using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using QuizWorld.Common.Claims;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Http;
using QuizWorld.Infrastructure.Data.Entities.Identity;


namespace QuizWorld.Infrastructure.AuthConfig.CanEditAndDeleteQuizzes
{
    public class CanEditAndDeleteQuizzesHandler(
        IHttpContextAccessor http,
        UserManager<ApplicationUser> userManager) 
            : AuthorizationHandler<CanEditAndDeleteQuizzesRequirement>
    {
        private readonly IHttpContextAccessor _http = http;
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEditAndDeleteQuizzesRequirement requirement)
        {
            if (context.HasSucceeded)
            {
                return;
            }

            ApplicationUser? user = await GetUser();
            string[] roles = await GetRolesAsync(user);
            bool hasRequiredRoles = Roles.HasRequiredRoles(roles, requirement.AllowedRoles);
            if (hasRequiredRoles)
            {
                AttachQuizToContextMiddlewareFlags flag = new()
                {
                    QuizId = GetRouteId(),
                    User = user?.UserName,
                    Method = _http.HttpContext?.Request.Method
                };

                if (_http.HttpContext is not null)
                {
                    _http.HttpContext.Items[AttachQuizToContextMiddlewareFlags.Metadata] = flag;

                    if (!IsCreatorOfQuiz())
                    {
                        _http.HttpContext.Items[AttachQuizToContextMiddlewareFlags.ShouldLogFlag] = true;
                    }

                    context.Succeed(requirement);
                    return;
                }

                return;
            }
        }

        public async Task Authorize(AuthorizationHandlerContext context, CanEditAndDeleteQuizzesRequirement requirement)
        {
            await HandleRequirementAsync(context, requirement);
        }

        private int GetRouteId()
        {
            var routeValue = _http.HttpContext?.GetRouteValue("id")?.ToString();
            if (routeValue is null)
            {
                return default;
            }

            int quizId = int.Parse(routeValue);
            return quizId;
        }

        private bool IsCreatorOfQuiz()
        {
            string? userId = _http.HttpContext?.User?.FindFirst(UserClaimsProperties.Id)?.Value;
            string? quizCreatorId = _http.HttpContext?.Items[AttachQuizToContextMiddlewareFlags.QuizCreatorIdFlag]?.ToString();

            return quizCreatorId is not null && userId == quizCreatorId;
        }

        private async Task<ApplicationUser?> GetUser()
        {
            string? userId = _http.HttpContext?.User?.FindFirst(UserClaimsProperties.Id)?.Value;
            if (userId is null)
            {
                return null;
            }

            ApplicationUser? appUser = await _userManager.FindByIdAsync(userId);
            return appUser;
        }

        private async Task<string[]> GetRolesAsync(ApplicationUser? user)
        {
            if (user is null)
            {
                return [];
            }

            var roles = await _userManager.GetRolesAsync(user);
            return [.. roles];
        }
    }
}
