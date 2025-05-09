using Microsoft.EntityFrameworkCore;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Infrastructure;
using QuizWorld.Web.Services.RoleService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Services.RoleServiceImMemoryTests
{
    public class InMemory
    {
        public TestDB testDb;
        public Repository repository;
        public RoleService service;

        [SetUp]
        public void Setup()
        {
            this.testDb = new TestDB();
            this.repository = this.testDb.repository;
            this.service = new RoleService(this.testDb.userManager);
        }

        [Test]
        public async Task Test_GetUsersOfRoleCorrectlyRetrievesUsersOfTheSpecifiedRole()
        {
            var result = await this.service.GetUsersOfRole(Roles.User, 1, SortingOrders.Ascending, 3);
            var users = result.Users.OrderBy(users => users.Username).ToArray();
            Assert.That(users, Has.Length.EqualTo(3));

            var admin = users[0];
            var moderator = users[1];
            var user = users[2];

            
            Assert.Multiple(() =>
            {
                Assert.That(admin.Roles, Does.Contain(Roles.Admin));
                Assert.That(admin.Roles, Does.Contain(Roles.Moderator));
                Assert.That(admin.Roles.Count, Is.EqualTo(3));

                Assert.That(moderator.Roles, Does.Contain(Roles.Moderator));
                Assert.That(moderator.Roles.Count, Is.EqualTo(2));
                Assert.That(user.Roles, Does.Contain(Roles.User));

                Assert.That(result.Total, Is.EqualTo(3));
            });

            var sortedResult = await this.service.GetUsersOfRole(Roles.Moderator, 2, SortingOrders.Ascending, 1);
            var user2 = sortedResult.Users.First();

            Assert.That(user2.Username, Is.EqualTo("moderator1"));
        }

        [Test]
        public async Task Test_GiveUserRoleSuccessfullyGivesTheUserARole()
        {
            var user = this.testDb.User;

            var result = await this.service.GiveUserRole(user.Id.ToString(), Roles.Moderator);
            Assert.That(result, Is.EqualTo(user.Id));

            var newModeratorRoles = await this.testDb.userManager.Users
                .Where(u => u.Id == user.Id)
                .Select(u => u.UserRoles.Select(ur => ur.Role.Name))
                .FirstAsync();

            var roles = newModeratorRoles.ToArray();

            Assert.That(roles, Does.Contain(Roles.Moderator));
            Assert.That(roles, Has.Length.EqualTo(2));
        }

        [Test]
        public async Task Test_GiveUserRoleReturnsNullIfItCannotGiveTheUserAValidRole()
        {
            var admin = this.testDb.Admin;
            var result = await this.service.GiveUserRole(admin.Id.ToString(), Roles.Moderator);
            Assert.That(result, Is.Null);

            var adminRoles = await this.testDb.userManager.Users
                .Where(u => u.Id == admin.Id)
                .Select(u => u.UserRoles.Select(ur => ur.Role.Name))
                .FirstAsync();

            var roles = adminRoles.ToArray();

            Assert.That(roles, Has.Length.EqualTo(3));
        }

        [Test]
        public async Task Test_RemoveRoleFromUserSuccessfullyRemovesTheRoleFromTheUser()
        {
            var admin = this.testDb.Admin;
            var result = await this.service.RemoveRoleFromUser(admin.Id.ToString(), Roles.Moderator);

            Assert.That(result, Is.EqualTo(admin.Id));

            var newAdminRoles = await this.testDb.userManager.Users
                .Where(u => u.Id == admin.Id)
                .Select(u => u.UserRoles.Select(ur => ur.Role.Name))
                .FirstAsync();

            var roles = newAdminRoles.ToArray();

            Assert.That(roles, Does.Not.Contain(Roles.Moderator));
            Assert.That(roles, Has.Length.EqualTo(2));
        }

        [Test]
        public async Task Test_RemoveRoleFromUserReturnsNullIfItCannotRemoveAValidRoleFromUser()
        {
            var user = this.testDb.User;
            var result = await this.service.RemoveRoleFromUser(user.Id.ToString(), Roles.Moderator);
            Assert.That(result, Is.Null);

            var userRoles = await this.testDb.userManager.Users
                .Where(u => u.Id == user.Id)
                .Select(u => u.UserRoles.Select(ur => ur.Role.Name))
                .FirstAsync();

            var roles = userRoles.ToArray();

            Assert.That(roles, Has.Length.EqualTo(1));
        }

        [Test]
        [TestCase("e", 1, 2, SortingOrders.Ascending, 1)]
        [TestCase("M", 2, 2, SortingOrders.Descending, 1)]
        public async Task Test_GetsUsersByUsernameCorrectlyRetrievesAListOfUsers(string query, int expectedTotal, int expectedRolesCount, SortingOrders order, int expectedAmountOfUsers)
        {
            var result = await this.service.GetUsersByUsername(query, 1, order, 1);
            Assert.Multiple(() =>
            {
                Assert.That(result.Users.Count, Is.EqualTo(expectedAmountOfUsers));
                Assert.That(result.Users.First().Roles.Count, Is.EqualTo(expectedRolesCount));
                Assert.That(result.Total, Is.EqualTo(expectedTotal));
            });
            
        }
    }
}
