using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using QuizWorld.Common.Claims;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Infrastructure.AuthConfig.Handlers;
using QuizWorld.Infrastructure.AuthConfig.Requirements;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Tests.Unit.Util;
using System.Security.Claims;

namespace Tests.Unit.Policy
{
    public class CanViewLogsHandlerTests
    {
        public DefaultHttpContext HttpContext { get; set; }
        public CanViewLogsHandler CanViewLogsHandler { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }
        public Guid UserId { get; set; }

        [SetUp]
        public void Setup()
        {
            HttpContext = new DefaultHttpContext();
            UserManager = MockUserManager.Get();
            CanViewLogsHandler = new CanViewLogsHandler(UserManager);
            UserId = Guid.NewGuid();
        }

        [Test]
        public async Task Test_SucceedsTheContextIfTheUserHasAllRoles()
        {
            string id = UserId.ToString();
            var requirement = new HasRolesRequirement(Roles.Admin, Roles.Moderator);
            var context = CreateAuthorizationHandlerContext(requirement, id);
            var user = new ApplicationUser();

            UserManager.FindByIdAsync(id).Returns(user);
            UserManager.GetRolesAsync(user).Returns([Roles.Admin, Roles.Moderator, Roles.User]);

            await CanViewLogsHandler.HandleAsync(context);

            Assert.That(context.HasSucceeded, Is.True);
        }

        [Test]
        public async Task Test_FailsTheContextIfTheUserMissesAtLeastOneRole()
        {
            string id = UserId.ToString();
            var requirement = new HasRolesRequirement(Roles.Admin, Roles.Moderator);
            var context = CreateAuthorizationHandlerContext(requirement, id);
            var user = new ApplicationUser();

            UserManager.FindByIdAsync(id).Returns(user);
            UserManager.GetRolesAsync(user).Returns([Roles.Moderator, Roles.User]);

            await CanViewLogsHandler.HandleAsync(context);

            Assert.That(context.HasFailed, Is.True);
        }

        [Test]
        public async Task Test_FailsTheContextIfTheUserCannotBeRetrievedFromClaims()
        {
            string? id = null;
            var requirement = new HasRolesRequirement(Roles.Admin, Roles.Moderator);
            var context = CreateAuthorizationHandlerContext(requirement, id);

            await CanViewLogsHandler.HandleAsync(context);

            Assert.That(context.HasFailed, Is.True);
        }

        [Test]
        public async Task Test_FailsTheContextIfTheUserCannotBeRetrievedFromTheDatabase()
        {
            string id = "test";
            var requirement = new HasRolesRequirement(Roles.Admin, Roles.Moderator);
            var context = CreateAuthorizationHandlerContext(requirement, id);

            UserManager.FindByIdAsync(id).Returns((ApplicationUser?)null);

            await CanViewLogsHandler.HandleAsync(context);

            Assert.That(context.HasFailed, Is.True);
        }


        [TearDown]
        public void Teardown()
        {
            UserManager.Dispose();
        }

        private static AuthorizationHandlerContext CreateAuthorizationHandlerContext(IAuthorizationRequirement requirement, string? userId)
        {
            if (userId is null)
            {
                return new AuthorizationHandlerContext([requirement], new ClaimsPrincipal(), null);
            }

            var claimsPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity([new Claim(UserClaimsProperties.Id, userId.ToString())]));


            return new AuthorizationHandlerContext([requirement], claimsPrincipal, null);
        }
    }
}
