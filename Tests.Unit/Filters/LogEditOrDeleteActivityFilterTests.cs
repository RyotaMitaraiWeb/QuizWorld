using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NSubstitute;
using QuizWorld.Common.Http;
using QuizWorld.Web;
using QuizWorld.Web.Contracts.Logging;
using QuizWorld.Web.Controllers;
using QuizWorld.Web.Filters;
using System.Net.Http;

namespace Tests.Unit.Filters
{
    public class LogEditOrDeleteActivityFilterTests
    {
        public IActivityLogger ActivityLogger { get; set; }
        public LogEditOrDeleteActivityFilter LogEditOrDeleteActivityFilter { get; set; }
        public ActionExecutingContext ActionExecutingContext { get; set; }
        public HttpContext HttpContext { get; set; }
        public ActionExecutionDelegate Next { get; set; }

        [SetUp]
        public void Setup()
        {
            ActivityLogger = Substitute.For<IActivityLogger>();

            HttpContext = new DefaultHttpContext();

            var actionContext = new ControllerContext
            {
                HttpContext = HttpContext,
                RouteData = new RouteData(),
                ActionDescriptor = new ControllerActionDescriptor(),
            };

            ActionExecutingContext = new ActionExecutingContext(
                actionContext,
                [],
                new Dictionary<string, object?>(),
                controller: new BaseController()
            );

            Next = () => Task.FromResult<ActionExecutedContext?>(null)!;

            LogEditOrDeleteActivityFilter = new LogEditOrDeleteActivityFilter(ActivityLogger);
        }

        [Test]
        public async Task Test_DoesNotCallTheLoggerWhenFlagsAreNotSet()
        {
            HttpContext.Items[AttachQuizToContextMiddlewareFlags.ShouldLogFlag] = true;
            HttpContext.Request.Method = HttpMethod.Put.Method;
            var metadata = new AttachQuizToContextMiddlewareFlags
            {
                Method = HttpMethod.Put.Method,
                QuizId = 1,
                User = "admin"
            };

            HttpContext.Items[AttachQuizToContextMiddlewareFlags.Metadata] = metadata;

            await LogEditOrDeleteActivityFilter.OnActionExecutionAsync(ActionExecutingContext, Next);

            await ActivityLogger.Received(1).LogActivity(Arg.Any<string>(), Arg.Any<DateTime>());
        }

        [Test]
        public async Task Test_CallsTheLoggerWhenAllFlagsAreSet()
        {
            HttpContext.Request.Method = HttpMethod.Put.Method;
            var metadata = new AttachQuizToContextMiddlewareFlags
            {
                Method = HttpMethod.Put.Method,
                QuizId = 1,
                User = "admin"
            };

            await LogEditOrDeleteActivityFilter.OnActionExecutionAsync(ActionExecutingContext, Next);

            await ActivityLogger.DidNotReceive().LogActivity(Arg.Any<string>(), Arg.Any<DateTime>());
        }
    }
}
