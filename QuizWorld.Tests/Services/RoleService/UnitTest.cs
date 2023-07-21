using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MockQueryable.Moq;
using Moq;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Infrastructure.Extensions;
using QuizWorld.Web.Services.RoleService;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Services.RoleServiceUnitTests
{
    public class UnitTest
    {
        public Mock<UserManager<ApplicationUser>> userManagerMock;
        public RoleService service;

        [SetUp]
        public void Setup()
        {
            this.userManagerMock = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object,
                new Mock<IOptions<IdentityOptions>>().Object,
                new Mock<IPasswordHasher<ApplicationUser>>().Object,
                new IUserValidator<ApplicationUser>[0],
                new IPasswordValidator<ApplicationUser>[0],
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<ILogger<UserManager<ApplicationUser>>>().Object);

            this.service = new RoleService(this.userManagerMock.Object);
        }


        [Test]
        public async Task Test_GetUsersOfRoleGetsUsersOfTheParticularRoleAndFormatsTheResultCorrectly()
        {
            this.userManagerMock
                .Setup(um => um.Users)
                .Returns(this.GenerateUsers());

            var result = await this.service.GetUsersOfRole(Roles.User, 1, SortingOrders.Ascending, 3);
            var users = result.ToArray();
            Assert.That(users, Has.Length.EqualTo(3));

            var admin = users[0];
            var moderator = users[1];
            var user = users[2];
            Assert.Multiple(() =>
            {
                Assert.That(admin.Username, Is.EqualTo("admin"));
                Assert.That(admin.Roles, Is.EqualTo("Administrator, Moderator"));
                Assert.That(moderator.Roles, Is.EqualTo("Moderator"));
                Assert.That(user.Roles, Is.EqualTo("User"));
            });
        }

        [Test]
        public async Task Test_GetUsersOfRoleThrowsArgumentExceptionIfRoleDoesNotExist()
        {
            try
            {
                var result = await this.service.GetUsersOfRole("Janitor", 1, SortingOrders.Ascending, 3);
                Assert.Fail("Method should have thrown but passed");
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }
            catch (Exception e)
            {
                Assert.Fail("Method did not throw an exception or threw a different type of exception. Message: " + e.Message);
            }

        }



        [Test]
        public async Task Test_GiveUserRoleReturnsTheGuidOfTheUserIfSuccessful()
        {
            var id = Guid.NewGuid();
            var user = new ApplicationUser()
            {
                Id = id,
            };

            this.userManagerMock
                .Setup(um => um.FindByIdAsync(id.ToString()))
                .ReturnsAsync(user);

            this.userManagerMock
                .Setup(um => um.AddToRoleAsync(user, Roles.Moderator))
                .ReturnsAsync(IdentityResult.Success);

            var result = await this.service.GiveUserRole(id.ToString(), Roles.Moderator);
            Assert.That(result, Is.EqualTo(id));
        }

        [Test]
        public async Task Test_GiveUserRoleWorksIfPassedAGuid()
        {
            var id = Guid.NewGuid();
            var user = new ApplicationUser()
            {
                Id = id,
            };

            this.userManagerMock
                .Setup(um => um.FindByIdAsync(id.ToString()))
                .ReturnsAsync(user);

            this.userManagerMock
                .Setup(um => um.AddToRoleAsync(user, Roles.Moderator))
                .ReturnsAsync(IdentityResult.Success);

            var result = await this.service.GiveUserRole(id, Roles.Moderator);
            Assert.That(result, Is.EqualTo(id));
        }

        [Test]
        public async Task Test_GiveUserRoleReturnsNullIfUserManagerReturnsIdentityResultFailed()
        {
            var id = new Guid();
            var user = new ApplicationUser()
            {
                Id = id,
            };

            this.userManagerMock
                .Setup(um => um.FindByIdAsync(id.ToString()))
                .ReturnsAsync(user);

            this.userManagerMock
                .Setup(um => um.AddToRoleAsync(user, Roles.Moderator))
                .ReturnsAsync(IdentityResult.Failed());

            var result = await this.service.GiveUserRole(id.ToString(), Roles.Moderator);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_GiveUserRoleReturnsNullIfUserManagerCannotFindTheUser()
        {
            var id = new Guid();
            this.userManagerMock
                .Setup(um => um.FindByIdAsync(id.ToString()))
                .ReturnsAsync(() => null);
            try
            {
                var result = await this.service.GiveUserRole(id.ToString(), Roles.Moderator);
                Assert.Fail("Method should have thrown but passed");
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }
            catch (Exception e)
            {
                Assert.Fail("Method did not throw an exception or threw a different type of exception. Message: " + e.Message);
            }
        }

        [Test]
        [TestCase("Janitor")]
        [TestCase(Roles.Admin)]
        public async Task Test_GiveUserRoleThrowsArgumentExceptionIfTheRoleIsNotAvailableToBeGiven(string role)
        {
            var id = new Guid();

            try
            {
                var result = await this.service.GiveUserRole(id.ToString(), role);
                Assert.Fail("Method should have thrown but passed");
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }
            catch (Exception e)
            {
                Assert.Fail("Method did not throw an exception or threw a different type of exception. Message: " + e.Message);
            }
        }

        [Test]
        public async Task Test_RemoveRoleFromUserReturnsTheGuidOfTheUserIfSuccessful()
        {
            var id = Guid.NewGuid();
            var user = new ApplicationUser()
            {
                Id = id,
            };

            this.userManagerMock
                .Setup(um => um.FindByIdAsync(id.ToString()))
                .ReturnsAsync(user);

            this.userManagerMock
                .Setup(um => um.RemoveFromRoleAsync(user, Roles.Moderator))
                .ReturnsAsync(IdentityResult.Success);

            var result = await this.service.RemoveRoleFromUser(id.ToString(), Roles.Moderator);
            Assert.That(result, Is.EqualTo(id));
        }

        [Test]
        public async Task Test_RemoveRoleFromUserWorksIfPassedAGuid()
        {
            var id = Guid.NewGuid();
            var user = new ApplicationUser()
            {
                Id = id,
            };

            this.userManagerMock
                .Setup(um => um.FindByIdAsync(id.ToString()))
                .ReturnsAsync(user);

            this.userManagerMock
                .Setup(um => um.RemoveFromRoleAsync(user, Roles.Moderator))
                .ReturnsAsync(IdentityResult.Success);

            var result = await this.service.RemoveRoleFromUser(id, Roles.Moderator);
            Assert.That(result, Is.EqualTo(id));
        }

        [Test]
        public async Task Test_RemoveRoleFromUserReturnsNullIfUserManagerReturnsIdentityResultFailed()
        {
            var id = new Guid();
            var user = new ApplicationUser()
            {
                Id = id,
            };

            this.userManagerMock
                .Setup(um => um.FindByIdAsync(id.ToString()))
                .ReturnsAsync(user);

            this.userManagerMock
                .Setup(um => um.RemoveFromRoleAsync(user, Roles.Moderator))
                .ReturnsAsync(IdentityResult.Failed());

            var result = await this.service.RemoveRoleFromUser(id.ToString(), Roles.Moderator);

            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_RemoveRoleFromUserReturnsNullIfUserManagerCannotFindTheUser()
        {
            var id = new Guid();
            this.userManagerMock
                .Setup(um => um.FindByIdAsync(id.ToString()))
                .ReturnsAsync(() => null);
            try
            {
                var result = await this.service.RemoveRoleFromUser(id.ToString(), Roles.Moderator);
                Assert.Fail("Method should have thrown but passed");
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }
            catch (Exception e)
            {
                Assert.Fail("Method did not throw an exception or threw a different type of exception. Message: " + e.Message);
            }
        }

        [Test]
        [TestCase("Janitor")]
        [TestCase(Roles.Admin)]
        public async Task Test_RemoveRoleFromUserThrowsArgumentExceptionIfTheRoleIsNotAvailableToBeGiven(string role)
        {
            var id = new Guid();

            try
            {
                var result = await this.service.RemoveRoleFromUser(id.ToString(), role);
                Assert.Fail("Method should have thrown but passed");
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }
            catch (Exception e)
            {
                Assert.Fail("Method did not throw an exception or threw a different type of exception. Message: " + e.Message);
            }
        }

        private IQueryable<ApplicationUser> GenerateUsers()
        {
            var users = new List<ApplicationUser>();

            var id1 = Guid.NewGuid();
            var id2 = Guid.NewGuid();
            var id3 = Guid.NewGuid();

            var userRole = new ApplicationRole() { Name = Roles.User, NormalizedName = Roles.User.Normalized() };
            var moderatorRole = new ApplicationRole() { Name = Roles.Moderator, NormalizedName = Roles.Moderator.Normalized() };
            var adminRole = new ApplicationRole() { Name = Roles.Admin, NormalizedName = Roles.Admin.Normalized() };

            var admin = new ApplicationUser()
            {
                Id = id1,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                UserRoles = new[]
                {
                    new ApplicationUserRole()
                    {
                        RoleId = adminRole.Id,
                        UserId = id1,
                        Role = adminRole,
                    },
                    new ApplicationUserRole()
                    {
                        RoleId = moderatorRole.Id,
                        UserId = id1,
                        Role = moderatorRole,
                    },
                    new ApplicationUserRole()
                    {
                        RoleId = userRole.Id,
                        UserId = id1,
                        Role = userRole,
                    }
                }
            };

            var moderator = new ApplicationUser()
            {
                Id = id2,
                UserName = "moderator",
                NormalizedUserName = "MODERATOR",
                UserRoles = new[]
                {
                    new ApplicationUserRole()
                    {
                        RoleId = moderatorRole.Id,
                        UserId = id2,
                        Role = moderatorRole,
                    },
                    new ApplicationUserRole()
                    {
                        RoleId = userRole.Id,
                        UserId = id2,
                        Role = userRole,
                    }
                }
            };

            var user = new ApplicationUser()
            {
                Id = id3,
                UserName = "ryota1",
                NormalizedUserName = "RYOTA1",
                UserRoles = new[]
                {
                    new ApplicationUserRole()
                    {
                        RoleId = userRole.Id,
                        UserId = id3,
                        Role = userRole,
                    }
                }
            };

            users.Add(admin);
            users.Add(moderator);
            users.Add(user);

            return users.BuildMock();
        }
    }
}
