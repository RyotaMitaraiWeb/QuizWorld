using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts;
using QuizWorld.Web.Services;

namespace QuizWorld.Tests.Services.UserServiceTest
{
    public class Tests
    {
        public Mock<UserManager<ApplicationUser>> userManagerMock;
        public UserService service;
        public RegisterViewModel register;
        public LoginViewModel login;
        public ApplicationUser user;
        public Mock<IConfiguration> config;

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

            this.config = new Mock<IConfiguration>();

            this.service = new UserService(this.userManagerMock.Object, this.config.Object);

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
        public void Test_GenerateJWTReturnsWhateverTokenIsGenerated()
        {
            var user = new UserViewModel()
            {
                Username = "ryota1",
                Id = Guid.NewGuid().ToString(),
                Roles = new string[] { "User" }
            };

            this.config
                .SetupGet(c => c["JWT:Secret"])
                .Returns("aswenwe12tasgq3qwsas3t");

            this.config
                .SetupGet(c => c["JWT:ValidAudience"])
                .Returns("localhost:4200");

            this.config
                .SetupGet(c => c["JWT:ValidIssuer"])
                .Returns("localhost:5000");

            var result = this.service.GenerateJWT(user);
            Assert.That(result, Has.Length.GreaterThan(20));
        }

        [Test]
        public void Test_DecodeJWTReturnsAUserViewModelWhenTokenIsValid()
        {
            var user = new UserViewModel()
            {
                Id = "qekonwe",
                Username = "ryota1",
                Roles = new string[] { "User" }
            };

            this.config
                .SetupGet(c => c["JWT:Secret"])
                .Returns("aswenwe12tasgq3qwsas3t");

            this.config
                .SetupGet(c => c["JWT:ValidAudience"])
                .Returns("localhost:4200");

            this.config
                .SetupGet(c => c["JWT:ValidIssuer"])
                .Returns("localhost:5000");

            var token = this.service.GenerateJWT(user);

            var result = this.service.DecodeJWT(token);

            Assert.Multiple(() =>
            {
                Assert.That(result.Id, Is.EqualTo(user.Id));
                Assert.That(result.Username, Is.EqualTo(user.Username));
                Assert.That(result.Roles[0], Is.EqualTo("User"));
                Assert.That(result.Roles, Has.Length.EqualTo(1));
            });
        }
    }
}
