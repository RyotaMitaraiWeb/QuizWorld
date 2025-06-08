using Bogus.DataSets;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Tests.Unit.Util;
using QuizWorld.ViewModels.Roles;
using QuizWorld.Web.Services;
using static QuizWorld.Common.Errors.RoleError;

namespace Tests.Unit.Services
{
    public class RoleServiceTests
    {
        public UserManager<ApplicationUser> UserManager { get; set; }
        public RoleService RoleService { get; set; }
        public ChangeRoleViewModel ChangeRoleData { get; set; }

        [SetUp]
        public void Setup()
        {
            UserManager = MockUserManager.Get();
            RoleService = new RoleService(UserManager);
            ChangeRoleData = new()
            {
                Role = Roles.Moderator,
                UserId = Guid.NewGuid().ToString(),
            };
        }

        [Test]
        public async Task Test_GiveUserRoleReturnsErrorWhenTheUserDoesNotExist()
        {
            UserManager.FindByIdAsync(ChangeRoleData.UserId)
                .Returns((ApplicationUser?)null);

            var result = await RoleService.GiveUserRole(ChangeRoleData);
            Assert.That(result.Error, Is.EqualTo(AddRoleError.UserDoesNotExist));
        }

        [Test]
        public async Task Test_GiveUserRoleReturnsErrorWhenTheRoleIsAlreadyGiven()
        {
            var user = CreateUser(Roles.Moderator);
            UserManager
                .FindByIdAsync(ChangeRoleData.UserId)
                .Returns(user);

            UserManager
                .GetRolesAsync(user)
                .Returns([Roles.Moderator]);

            var result = await RoleService.GiveUserRole(ChangeRoleData);
            Assert.That(result.Error, Is.EqualTo(AddRoleError.UserAlreadyHasRole));
        }

        [Test]
        public async Task Test_GiveUserRoleReturnsSuccessWhenSuccessful()
        {
            var user = CreateUser(Roles.User);
            UserManager
                .FindByIdAsync(ChangeRoleData.UserId)
                .Returns(user);

            UserManager
                .GetRolesAsync(user)
                .Returns([Roles.User]);

            var result = await RoleService.GiveUserRole(ChangeRoleData);
            Assert.That(result.IsSuccess, Is.True);
        }

        [TearDown]
        public void Teardown()
        {
            UserManager.Dispose();
        }

        private ApplicationUser CreateUser(params string[] roles)
        {
            ApplicationUser user = new()
            {
                Id = new Guid(ChangeRoleData.UserId),
                UserRoles = [.. roles.Select(role => new ApplicationUserRole()
                {
                    Role = new ApplicationRole() {  Name = role }
                })]
            };

            return user;
        }
    }
}
