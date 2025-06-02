using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Web.Filters;
using Microsoft.AspNetCore.Routing;

namespace Tests.Unit.Filters
{
    public class NotFoundExceptionAttributeTests
    {
        [Test]
        public void Test_ReturnsANotFoundErrorIfTheActionThrows()
        {
            var filter = new NotFoundExceptionAttribute();

            var httpContext = new DefaultHttpContext();
            var routeData = new RouteData();
            var actionDescriptor = new ControllerActionDescriptor();
            var actionContext = new ActionContext(httpContext, routeData, actionDescriptor);

            var exceptionContext = new ExceptionContext(
                actionContext,
                [])
            {
                Exception = new Exception("Test exception")
            };

            filter.OnException(exceptionContext);

            Assert.Multiple(() =>
            {
                Assert.That(exceptionContext.Result, Is.TypeOf<NotFoundResult>());
                Assert.That(exceptionContext.ExceptionHandled, Is.True);
            });
        }
    }
}
