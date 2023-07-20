using QuizWorld.Common.Constants.Roles;
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
            var result = await this.service.GetUsersOfRole(Roles.User, 1, 3);
            var users = result.OrderBy(users => users.Username).ToArray();
            Assert.That(users, Has.Length.EqualTo(3));

            var admin = users[0];
            var moderator = users[1];
            var user = users[2];

            Assert.That(admin.Roles, Is.EqualTo("Administrator, Moderator"));
            Assert.Multiple(() =>
            {
                Assert.That(moderator.Roles, Is.EqualTo("Moderator"));
                Assert.That(user.Roles, Is.EqualTo("User"));
            });

            var sortedResult = await this.service.GetUsersOfRole(Roles.Moderator, 2, 1);
            var user2 = sortedResult.First();

            Assert.That(user2.Username, Is.EqualTo("moderator1"));
        }
    }
}
