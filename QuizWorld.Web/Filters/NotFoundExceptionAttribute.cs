using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace QuizWorld.Web.Filters
{
    /// <summary>
    /// When a given endpoint / action throws an exception, this filter will make
    /// it return a 404 error instead of the usual error. This can be used
    /// in cases where the existence of an endpoint isn't public info
    /// (such as the admin area) and is thus too sensitive to be exposed
    /// to your normal user.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class NotFoundExceptionAttribute : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            context.Result = new NotFoundResult();
            context.ExceptionHandled = true;
        }
    }
}
