using Microsoft.AspNetCore.Identity;
using NSubstitute;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.Tests.Unit.Util;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Services;
using static QuizWorld.Common.Errors.AuthError;

namespace Tests.Unit.Services
{
    public class AuthServiceTests
    {
        public AuthService AuthService { get; set; }
        public UserManager<ApplicationUser> UserManager { get; set; }
        public LoginViewModel ExampleLogin = new ()
        {
            Username = "admin",
            Password = "123456",
        };

        public RegisterViewModel ExampleRegister = new()
        {
            Username = "admin",
            Password = "123456",
        };

        public ApplicationUser ExampleUser
        {
            get
            {
                return new ApplicationUser()
                {
                    Id = Guid.NewGuid(),
                    UserName = ExampleLogin.Username,
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            UserManager = MockUserManager.Get();
            AuthService = new AuthService(UserManager);
        }

        [Test]
        
        public async Task Test_LoginReturnsErrorWhenUserDoesNotExist()
        {
            UserManager.FindByNameAsync(ExampleLogin.Username)
                .Returns((ApplicationUser?) null);

            var result = await AuthService.LoginAsync(ExampleLogin);
            Assert.That(result.Error, Is.EqualTo(FailedLoginError.UserDoesNotExist));
        }

        [Test]
        public async Task Test_LoginReturnsErrorWhenPasswordIsIncorrect()
        {
            var user = ExampleUser;
            UserManager.FindByNameAsync(ExampleLogin.Username)
                .Returns(user);

            UserManager.CheckPasswordAsync(user, ExampleLogin.Password)
                .Returns(false);

            var result = await AuthService.LoginAsync(ExampleLogin);
            Assert.That(result.Error, Is.EqualTo(FailedLoginError.WrongPassword));
        }

        [Test]
        public async Task Test_LoginReturnsAUserWhenSuccessful()
        {
            var user = ExampleUser;
            UserManager.FindByNameAsync(ExampleLogin.Username)
                .Returns(user);

            UserManager.CheckPasswordAsync(user, ExampleLogin.Password)
                .Returns(true);

            UserManager.GetRolesAsync(user)
                .Returns([Roles.User]);

            var result = await AuthService.LoginAsync(ExampleLogin);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Value.Id, Is.EqualTo(user.Id.ToString()));
                Assert.That(result.Value.Username, Is.EqualTo(user.UserName));
            });
        }

        [Test]
        public async Task Test_RegisterReturnsUsernameIsTakenErrorIfItFindsAnExistingUser()
        {
            UserManager.FindByNameAsync(ExampleRegister.Username)
                .Returns(new ApplicationUser());

            var result = await AuthService.RegisterAsync(ExampleRegister);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailure, Is.True);
                Assert.That(result.Error, Is.EqualTo(FailedRegisterError.UsernameIsTaken));
            });
        }

        [Test]
        public async Task Test_RegisterReturnsFailErrorIfSomethingGoesWrongWhileCreatingTheUser()
        {
            UserManager.FindByNameAsync(ExampleRegister.Username)
                .Returns((ApplicationUser?)null);

            UserManager.CreateAsync(new ApplicationUser(), ExampleRegister.Password)
                .ReturnsForAnyArgs(IdentityResult.Failed());

            var result = await AuthService.RegisterAsync(ExampleRegister);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailure, Is.True);
                Assert.That(result.Error, Is.EqualTo(FailedRegisterError.Fail));
            });
        }

        [Test]
        public async Task Test_RegisterReturnsFailErrorIfSomethingGoesWrongWhileAddingDefaultRoles()
        {
            UserManager.FindByNameAsync(ExampleRegister.Username)
                .Returns((ApplicationUser?)null);

            UserManager.CreateAsync(new ApplicationUser(), ExampleRegister.Password)
                .ReturnsForAnyArgs(IdentityResult.Success);

            UserManager.AddToRoleAsync(new ApplicationUser(), Roles.User)
                .ReturnsForAnyArgs(IdentityResult.Failed());

            var result = await AuthService.RegisterAsync(ExampleRegister);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsFailure, Is.True);
                Assert.That(result.Error, Is.EqualTo(FailedRegisterError.Fail));
            });
        }

        [Test]
        public async Task Test_RegisterReturnsAUserWhenSuccessful()
        {
            UserManager.FindByNameAsync(ExampleRegister.Username)
                .Returns((ApplicationUser?)null);

            UserManager.CreateAsync(new ApplicationUser(), ExampleRegister.Password)
                .ReturnsForAnyArgs(IdentityResult.Success);

            UserManager.AddToRoleAsync(new ApplicationUser(), Roles.User)
                .ReturnsForAnyArgs(IdentityResult.Success);

            var result = await AuthService.RegisterAsync(ExampleRegister);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Value.Username, Is.EqualTo(ExampleRegister.Username));
            });
        }

        [Test]
        public async Task Test_CheckIfUsernameReturnsTrueIfUserExists()
        {
            string username = "a";
            UserManager.FindByNameAsync(username)
                .Returns(new ApplicationUser());

            var result = await AuthService.CheckIfUsernameIsTakenAsync(username);

            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Test_CheckIfUsernameReturnsFalseIfUserDoesNotExist()
        {
            string username = "a";
            UserManager.FindByNameAsync(username)
                .Returns((ApplicationUser?)null);


            var result = await AuthService.CheckIfUsernameIsTakenAsync(username);

            Assert.That(result, Is.False);
        }

        [TearDown]
        public void TearDown()
        {
            UserManager.Dispose();
        }
    }
}
