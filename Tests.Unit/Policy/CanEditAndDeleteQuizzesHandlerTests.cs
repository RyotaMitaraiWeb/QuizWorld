using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using QuizWorld.Common.Claims;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Http;
using QuizWorld.Infrastructure.AuthConfig.CanEditAndDeleteQuizzes;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Tests.Unit.Util;
using System.Security.Claims;

namespace Tests.Unit.Policy
{
    public class CanEditAndDeleteQuizzesHandlerTests
    {
        public DefaultHttpContext HttpContext { get; set; }
        public IHttpContextAccessor HttpContextAccessor { get; set; }
        public CanEditAndDeleteQuizzesHandler CanEditAndDeleteQuizzesHandler { get; set; }
        public CanEditAndDeleteQuizzesRequirement CanEditAndDeleteQuizzesRequirement { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }
        public Guid CreatorId { get; set; }
        public int QuizId = 1;

        [SetUp]
        public void Setup()
        {
            CreatorId = Guid.NewGuid();
            HttpContext = new DefaultHttpContext();
            HttpContext.Request.RouteValues["id"] = QuizId;
            HttpContext.Request.Method = HttpMethod.Put.Method;
            HttpContextAccessor = Substitute.For<IHttpContextAccessor>();
            HttpContextAccessor.HttpContext.Returns(HttpContext);
            UserManager = MockUserManager.Get();
            CanEditAndDeleteQuizzesHandler = new CanEditAndDeleteQuizzesHandler(HttpContextAccessor, UserManager);
            CanEditAndDeleteQuizzesRequirement = new CanEditAndDeleteQuizzesRequirement(Roles.Moderator, Roles.Admin);
        }

        [Test]
        public async Task Test_SucceedsTheContextIfTheUserHasTheRequiredRolesAndIsNotTheCreator()
        {
            var userId = Guid.NewGuid();
            HttpContext.Items[AttachQuizToContextMiddlewareFlags.QuizCreatorIdFlag] = CreatorId.ToString();
            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity([new Claim(UserClaimsProperties.Id, userId.ToString())]));

            HttpContext.User = claimsPrincipal;

            var user = new ApplicationUser()
            {
                Id = userId,
                UserName = "admin"
            };

            UserManager.FindByIdAsync(userId.ToString()).Returns(user);
            UserManager.GetRolesAsync(user).Returns([Roles.Admin, Roles.Moderator, Roles.User]);

            var context = new AuthorizationHandlerContext([CanEditAndDeleteQuizzesRequirement], claimsPrincipal, null);

            await CanEditAndDeleteQuizzesHandler.Authorize(context, CanEditAndDeleteQuizzesRequirement);
            Assert.Multiple(() =>
            {
                Assert.That(context.HasSucceeded, Is.True);
                Assert.That(HttpContext.Items[AttachQuizToContextMiddlewareFlags.ShouldLogFlag], Is.Not.Null);
            });
        }

        [Test]
        public async Task Test_SucceedsTheContextIfTheUserHasTheRequiredRolesAndIsTheCreator()
        {
            HttpContext.Items[AttachQuizToContextMiddlewareFlags.QuizCreatorIdFlag] = CreatorId.ToString();
            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity([new Claim(UserClaimsProperties.Id, CreatorId.ToString())]));

            HttpContext.User = claimsPrincipal;

            var user = new ApplicationUser()
            {
                Id = CreatorId,
                UserName = "admin"
            };

            UserManager.FindByIdAsync(CreatorId.ToString()).Returns(user);
            UserManager.GetRolesAsync(user).Returns([Roles.Admin, Roles.Moderator, Roles.User]);

            var context = new AuthorizationHandlerContext([CanEditAndDeleteQuizzesRequirement], claimsPrincipal, null);

            await CanEditAndDeleteQuizzesHandler.Authorize(context, CanEditAndDeleteQuizzesRequirement);
            Assert.Multiple(() =>
            {
                Assert.That(context.HasSucceeded, Is.True);
                Assert.That(HttpContext.Items[AttachQuizToContextMiddlewareFlags.ShouldLogFlag], Is.Null);
            });
        }

        [Test]
        public async Task Test_DoesNotSucceedTheContextIfTheUserIsMissingAtLeastOneRole()
        {
            HttpContext.Items[AttachQuizToContextMiddlewareFlags.QuizCreatorIdFlag] = CreatorId.ToString();
            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity([new Claim(UserClaimsProperties.Id, CreatorId.ToString())]));

            HttpContext.User = claimsPrincipal;

            var user = new ApplicationUser()
            {
                Id = CreatorId,
                UserName = "admin"
            };

            UserManager.FindByIdAsync(CreatorId.ToString()).Returns(user);
            UserManager.GetRolesAsync(user).Returns([Roles.Moderator, Roles.User]);

            var context = new AuthorizationHandlerContext([CanEditAndDeleteQuizzesRequirement], claimsPrincipal, null);

            await CanEditAndDeleteQuizzesHandler.Authorize(context, CanEditAndDeleteQuizzesRequirement);

            Assert.That(context.HasSucceeded, Is.False);
        }

        [TearDown]
        public void Teardown()
        {
            UserManager.Dispose();
        }
    }
}
