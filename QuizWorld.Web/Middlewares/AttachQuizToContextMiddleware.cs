
using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Http;
using QuizWorld.Infrastructure;
using QuizWorld.Infrastructure.Data.Entities.Quiz;
using QuizWorld.Web.Controllers;

namespace QuizWorld.Web.Middlewares
{
    public class AttachQuizToContextMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate next = next;
        public async Task InvokeAsync(HttpContext context, IRepository repository)
        {
            if (RouteMatches(context))
            {
                var routeValues = context.Request.RouteValues;
                if (routeValues.TryGetValue("id", out var idObj) && int.TryParse(idObj?.ToString(), out int id))
                {
                    string? creatorId = await repository
                        .AllReadonly<Quiz>(quiz => quiz.Id == id && !quiz.IsDeleted)
                        .Select(quiz => quiz.CreatorId.ToString())
                        .FirstOrDefaultAsync();

                    if (creatorId is null)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        return;
                    }

                    context.Items[AttachQuizToContextMiddlewareFlags.QuizCreatorIdFlag] = creatorId;
                }
            }

            await next(context);
        }

        private static bool RouteMatches(HttpContext context)
        {
            return IsPutOrDeleteRequest(context) || IsRequestForGetForEdit(context);
        }

        private static bool IsPutOrDeleteRequest(HttpContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;

            return path.StartsWithSegments("/quiz") 
                && QuizController.CanEditAndDeleteQuizzesEndpointMethods.Contains(method);
        }

        public static bool IsRequestForGetForEdit(HttpContext context)
        {
            var path = context.Request.Path;
            var method = context.Request.Method;

            return (path.Value?.EndsWith(QuizController.GetQuizForEditEndpointEnding) ?? false)
                && method == HttpMethod.Get.Method;
        }
    }

    public static class AttachQuizToContextMiddlewareExtensions
    {
        public static IApplicationBuilder UseAttachQuizToContext(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AttachQuizToContextMiddleware>();
        }
    }
}
