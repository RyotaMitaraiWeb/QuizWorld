using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using QuizWorld.Common.Claims;
using QuizWorld.Common.Http;
using QuizWorld.Infrastructure.AuthConfig.Handlers;
using QuizWorld.Infrastructure.AuthConfig.Requirements;
using System.Security.Claims;


namespace Tests.Unit.Policy
{
    public class CreatedTheQuizHandlerTests
    {
        public DefaultHttpContext HttpContext;
        public IHttpContextAccessor HttpContextAccessor;
        public CreatedTheQuizHandler CreatedTheQuizHandler;
        public CanEditAndDeleteQuizzesRequirement CanEditAndDeleteQuizzesRequirement;

        [SetUp]
        public void Setup()
        {
            HttpContext = new DefaultHttpContext();
            HttpContextAccessor = Substitute.For<IHttpContextAccessor>();
            HttpContextAccessor.HttpContext.Returns(HttpContext);

            CreatedTheQuizHandler = new CreatedTheQuizHandler(HttpContextAccessor);
            CanEditAndDeleteQuizzesRequirement = new CanEditAndDeleteQuizzesRequirement();
        }

        [Test]
        public async Task Test_WorksCorrectlyWhenTheUserHasCreatedTheQuiz()
        {
            var userId = Guid.NewGuid().ToString();
            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity([new Claim(UserClaimsProperties.Id, userId)]));

            HttpContext.User = claimsPrincipal;
            HttpContext.Items[AttachQuizToContextMiddlewareFlags.QuizCreatorIdFlag] = userId;

            var context = new AuthorizationHandlerContext([CanEditAndDeleteQuizzesRequirement], claimsPrincipal, null);

            await CreatedTheQuizHandler.Authorize(context, CanEditAndDeleteQuizzesRequirement);

            Assert.That(context.HasSucceeded, Is.True);
        }

        [Test]
        public async Task Test_WorksCorrectlyWhenTheUserHasNotCreatedTheQuiz()
        {
            var userId = Guid.NewGuid().ToString();
            var creatorId = Guid.NewGuid().ToString();
            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity([new Claim(UserClaimsProperties.Id, userId)]));

            HttpContext.User = claimsPrincipal;
            HttpContext.Items[AttachQuizToContextMiddlewareFlags.QuizCreatorIdFlag] = creatorId;

            var context = new AuthorizationHandlerContext([CanEditAndDeleteQuizzesRequirement], claimsPrincipal, null);

            await CreatedTheQuizHandler.Authorize(context, CanEditAndDeleteQuizzesRequirement);

            Assert.That(context.HasSucceeded, Is.False);
        }
    }
}
