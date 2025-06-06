using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using NUnit.Framework;
using QuizWorld.Web.Middlewares;

namespace Tests.Unit.Middlewares
{
    public class To404NotFoundMiddlewareResultHandlerTests
    {
        public To404NotFoundMiddlewareResultHandler To404NotFoundMiddlewareResultHandler;
        public AuthorizationMiddlewareResultHandler DefaultHandler;
        public DefaultHttpContext HttpContext;
        public RequestDelegate next;
        public AuthorizationPolicy Policy;
        public AuthorizationFailure Failure { get; set; }

        [SetUp]
        public void SetUp()
        {
            DefaultHandler = Substitute.For<AuthorizationMiddlewareResultHandler>();
            To404NotFoundMiddlewareResultHandler = new To404NotFoundMiddlewareResultHandler(DefaultHandler);
            HttpContext = new DefaultHttpContext();
            next = Substitute.For<RequestDelegate>();
            Policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
            Failure = AuthorizationFailure.ExplicitFail();
        }

        [TestCase("/logs/view")]
        [TestCase("/roles/manage")]
        public async Task Test_FailingAuth_OnMatchingPath_Returns404(string path)
        {
            HttpContext.Request.Path = path;
            var result = PolicyAuthorizationResult.Forbid(Failure);

            await To404NotFoundMiddlewareResultHandler.HandleAsync(next, HttpContext, Policy, result);

            Assert.That(HttpContext.Response.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task HandleAsync_Success_DelegatesToDefaultHandler()
        {
            HttpContext.Request.Path = "/logs";
            var result = PolicyAuthorizationResult.Success();

            await To404NotFoundMiddlewareResultHandler.HandleAsync(next, HttpContext, Policy, result);
            Assert.That(HttpContext.Response.StatusCode, Is.Not.EqualTo(404));
        }
    }
}