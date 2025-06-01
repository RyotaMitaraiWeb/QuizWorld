using Microsoft.AspNetCore.Mvc.Filters;
using QuizWorld.Common.Http;
using QuizWorld.Web.Contracts.Logging;

namespace QuizWorld.Web.Filters
{
    public class LogEditOrDeleteActivityFilter(IActivityLogger activityLogger) : IAsyncActionFilter
    {
        private readonly IActivityLogger _activityLogger = activityLogger;

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
           await next.Invoke();
           await OnActionExecutedAsync(context);
        }

        private async Task OnActionExecutedAsync(ActionExecutingContext context)
        {
            object? shouldLog = context.HttpContext?.Items[AttachQuizToContextMiddlewareFlags.ShouldLogFlag];
            
            if (shouldLog is null) return;

            if (context.HttpContext?.Items[AttachQuizToContextMiddlewareFlags.Metadata]
                is not AttachQuizToContextMiddlewareFlags flags) return;

            string message = BuildMessage(flags);
            await _activityLogger.LogActivity(message, DateTime.Now);
        }

        private static string BuildMessage(AttachQuizToContextMiddlewareFlags flags)
        {
            string action = DetermineAction(flags.Method);
            return $"User \"{flags.User}\" {action} a quiz with ID {flags.QuizId}";
        }

        private static string DetermineAction(string? method)
        {
            if (method is null) return string.Empty;
            return MethodToAction[method];
        }

        private static readonly Dictionary<string, string> MethodToAction = new()
        {
            { HttpMethod.Put.Method, "edited" },
            { HttpMethod.Delete.Method, "deleted" },
        };
    }
}
