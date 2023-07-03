using Microsoft.AspNetCore.Identity;
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
        public ApplicationUser user;

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

            this.service = new UserService(this.userManagerMock.Object);
            this.register = new RegisterViewModel()
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
    }
}
