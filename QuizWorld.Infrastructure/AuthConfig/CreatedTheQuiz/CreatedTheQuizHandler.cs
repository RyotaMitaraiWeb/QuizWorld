using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using QuizWorld.Common.Claims;
using QuizWorld.Common.Http;
using QuizWorld.Infrastructure.AuthConfig.CanEditAndDeleteQuizzes;

namespace QuizWorld.Infrastructure.AuthConfig.CreatedTheQuiz
{
    public class CreatedTheQuizHandler(IHttpContextAccessor http) : AuthorizationHandler<CanEditAndDeleteQuizzesRequirement>
    {
        private readonly IHttpContextAccessor _http = http;
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CanEditAndDeleteQuizzesRequirement requirement)
        {
            if (context.HasSucceeded)
            {
                return Task.CompletedTask;
            }

            if (IsCreatorOfQuiz())
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

        public async Task Authorize(AuthorizationHandlerContext context, CanEditAndDeleteQuizzesRequirement requirement)
        {
            await HandleRequirementAsync(context, requirement);
        }

        private bool IsCreatorOfQuiz()
        {
            string? userId = _http.HttpContext?.User?.FindFirst(UserClaimsProperties.Id)?.Value;
            string? quizCreatorId = _http.HttpContext?.Items[AttachQuizToContextMiddlewareFlags.QuizCreatorIdFlag]?.ToString();

            bool userIdIsGuid = Guid.TryParse(userId, out Guid userIdGuid);
            bool quizCreatorIdIsGuid = Guid.TryParse(quizCreatorId, out Guid quizCreatorIdGuid);

            if (!userIdIsGuid || !quizCreatorIdIsGuid)
            {
                return false;
            }

            return quizCreatorIdGuid == userIdGuid;
        }
    }
}
