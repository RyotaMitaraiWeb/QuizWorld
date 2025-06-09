using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using QuizWorld.Web.Contracts.JsonWebToken;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Web.Services.Legacy;

namespace QuizWorld.Tests.Services.UserServiceTest
{
    public class Tests
    {
        public Mock<UserManager<ApplicationUser>> userManagerMock;
        public UserService service;
        public RegisterViewModel register;
        public LoginViewModel login;
        public ApplicationUser user;
        public Mock<IJwtServiceDeprecated> jwtServiceMock;

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

            this.jwtServiceMock = new Mock<IJwtServiceDeprecated>();

            this.service = new UserService(this.userManagerMock.Object, this.jwtServiceMock.Object);

            this.register = new RegisterViewModel()
            {
                Username = "ryota1",
                Password = "123456",
            };

            this.login = new LoginViewModel()
            {
                Username = "ryota1",
                Password = "123456",
            };

            this.user = new ApplicationUser()
            {
                UserName = "ryota1",
                PasswordHash = "q123123623",
                NormalizedUserName = "RYOTA1",
            };
        }

        [Test]
        public async Task Test_RegisterReturnsAUserWhenUserManagerRegistersThemSuccessfully()
        {
            this.userManagerMock
                .Setup(um => um.CreateAsync(It.IsAny<ApplicationUser>(), this.register.Password))
                .ReturnsAsync(IdentityResult.Success);

            this.userManagerMock
                .Setup(um => um.AddToRoleAsync(this.user, "User"))
                .ReturnsAsync(IdentityResult.Success);

            var result = await this.service.Register(this.register);
            Assert.Multiple(() =>
            {
                Assert.That(result?.Username, Is.EqualTo(this.register.Username));
                Assert.That(result?.Roles[0], Is.EqualTo("User"));
                Assert.That(result?.Roles.Length, Is.EqualTo(1));
            });
        }

        [Test]
        public async Task Test_RegisterReturnsNullIfItCannotRegisterTheUser()
        {
            this.userManagerMock
                .Setup(um => um.CreateAsync(this.user, this.register.Password))
                .ReturnsAsync(IdentityResult.Failed
                (
                    new IdentityError
                    {
                        Code = "Error",
                        Description = "Error",
                    }
                )
            );

            var result = await this.service.Register(this.register);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_LoginReturnsAUserWhenUserManagerCheckPasswordReturnsTrue()
        {
            this.userManagerMock
                .Setup(um => um.FindByNameAsync(this.login.Username))
                .ReturnsAsync(new ApplicationUser()
                {
                    UserName = this.login.Username
                });

            this.userManagerMock
                .Setup(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), this.login.Password))
                .ReturnsAsync(true);

            this.userManagerMock
                .Setup(um => um.GetRolesAsync(It.IsAny<ApplicationUser>()))
                .ReturnsAsync(new List<string>() { "User" });

            var result = await this.service.Login(this.login);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Username, Is.EqualTo(this.login.Username));
            });
        }

        [Test]
        public async Task Test_LoginReturnsNullWhenUserManagerCheckPasswordReturnsFalse()
        {
            this.userManagerMock
                .Setup(um => um.FindByNameAsync(this.login.Username))
                .ReturnsAsync(new ApplicationUser());

            this.userManagerMock
                .Setup(um => um.CheckPasswordAsync(It.IsAny<ApplicationUser>(), this.login.Password))
                .ReturnsAsync(false);

            var result = await this.service.Login(this.login);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_LoginReturnsNullWhenUserManagerCannotFindTheUser()
        {
            this.userManagerMock
                .Setup(um => um.FindByNameAsync(this.login.Username))
                .ReturnsAsync(() => null);

            var result = await this.service.Login(this.login);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_LogoutReturnsTrueIfBlacklistJWTReturnsTrue()
        {
            string jwt = "a";
            this.jwtServiceMock
                .Setup(j => j.InvalidateJWT(jwt))
                .ReturnsAsync(true);

            var result = await this.service.Logout(jwt);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Test_LogoutReturnsFalseIfBlacklistJWTReturnsFalse()
        {
            string jwt = "a";
            this.jwtServiceMock
                .Setup(j => j.InvalidateJWT(jwt))
                .ReturnsAsync(false);

            var result = await this.service.Logout(jwt);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task Test_CheckIfUsernameIsTakenReturnsTrueIfTheUserManagerFindsAUserWithTheGivenUsername()
        {
            this.userManagerMock
                .Setup(um => um.FindByNameAsync("a"))
                .ReturnsAsync(new ApplicationUser());

            var result = await this.service.CheckIfUsernameIsTaken("a");
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Test_CheckIfUsernameIsTakenReturnsFalseIfTheUserManagerDoesNotFindAUserWithTheGivenUsername()
        {
            this.userManagerMock
                .Setup(um => um.FindByNameAsync("a"))
                .ReturnsAsync(() => null);

            var result = await this.service.CheckIfUsernameIsTaken("a");
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task Test_GetUserByUsernameReturnsNullIfTheUserDoesNotExist()
        {
            this.userManagerMock
                .Setup(um => um.FindByNameAsync("a"))
                .ReturnsAsync(() => null);

            var result = await this.service.GetUserByUsername("a");
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_GetUserByUsernameReturnsAUserViewModel()
        {
            var appUser = new ApplicationUser()
            {
                UserName = "ryota",
                Id = Guid.NewGuid(),
            };

            this.userManagerMock
                .Setup(um => um.FindByNameAsync("a"))
                .ReturnsAsync(appUser);

            this.userManagerMock
                .Setup(um => um.GetRolesAsync(appUser))
                .ReturnsAsync([Roles.User]);

            var result = await this.service.GetUserByUsername("a");
            Assert.That(result.Username, Is.EqualTo(appUser.UserName));
        }
    }
}
