using Microsoft.AspNetCore.Mvc.Filters;
using QuizWorld.Common.Claims;
using QuizWorld.ViewModels.Roles;
using QuizWorld.Web.Contracts.Logging;
using System.Text;

namespace QuizWorld.Web.Filters
{
    public class LogRoleChangeFilter(IActivityLogger activityLogger) : IAsyncActionFilter
    {
        private readonly IActivityLogger _activityLogger = activityLogger;
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            await next();
            var test = context.ActionArguments.Values.FirstOrDefault(aa => aa is ChangeRoleViewModel) as ChangeRoleViewModel;
            
            if (test is not null)
            {
                string username = GetUsername(context) ?? string.Empty;
                var now = DateTime.Now;

                if (IsTryingToAddRole(context))
                {
                    await _activityLogger.LogActivity(
                        $"User \"{username}\" tried to give role \"{test.Role}\" to a user with ID {test.UserId}",
                        now);
                } else
                {
                    await _activityLogger.LogActivity(
                        $"User \"{username}\" tried to remove role \"{test.Role}\" from a user with ID {test.UserId}",
                        now);
                }
            }


        }

        private static string? GetUsername(ActionExecutingContext context)
        {
            string? username = context.HttpContext.User.FindFirst(UserClaimsProperties.Username)?.Value;
            return username;
        }

        private static bool IsTryingToAddRole(ActionExecutingContext context)
        {
            var request = context.HttpContext.Request;
            return request.Path.ToString().EndsWith("add");
        }
    }
}
