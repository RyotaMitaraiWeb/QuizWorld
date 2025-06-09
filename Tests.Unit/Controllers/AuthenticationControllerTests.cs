using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Http;
using QuizWorld.Common.Result;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Areas.Authentication.Controllers;
using QuizWorld.Web.Contracts;
using static QuizWorld.Common.Errors.AuthError;
using static QuizWorld.Common.Results.JwtError;
using static QuizWorld.Common.Results.JwtStoreError;

namespace QuizWorld.Tests.Unit.Controllers
{
    public class AuthenticationControllerTests
    {
        public IJwtService JwtService { get; set; }
        public IAuthService AuthService { get; set; }
        public IJwtStore JwtStore { get; set; }
        public AuthenticationController AuthenticationController { get; set; }
        public UserViewModel ExampleUser = new()
        {
            Id = Guid.NewGuid().ToString(),
            Username = "admin",
            Roles = [Roles.User],
        };

        public LoginViewModel ExampleLogin
        {
            get
            {
                return new LoginViewModel()
                {
                    Username = ExampleUser.Username,
                    Password = "123456",
                };
            }
        }

        public RegisterViewModel ExampleRegister
        {
            get
            {
                return new RegisterViewModel()
                {
                    Username = ExampleUser.Username,
                    Password = "123456",
                };
            }
        }

        [SetUp]
        public void Setup()
        {
            JwtService = Substitute.For<IJwtService>();
            AuthService = Substitute.For<IAuthService>();
            JwtStore = Substitute.For<IJwtStore>();
            AuthenticationController = new AuthenticationController(
                JwtService,
                AuthService,
                JwtStore
            );
        }

        [Test]
        [TestCase(" ")]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase(null)]
        public async Task Test_RetrieveSessionReturnsUnauthorizedIfBearerIsNullOrWhitespace(string? nullOrWhitespaceBearer)
        {
            var result = await AuthenticationController.RetrieveSession(nullOrWhitespaceBearer);
            var response = result as UnauthorizedResult;
            Assert.That(response?.StatusCode, Is.EqualTo(401));
        }

        [Test]
        [TestCase(ExtractUserFromTokenErrors.Invalid)]
        [TestCase(ExtractUserFromTokenErrors.Expired)]
        public async Task Test_RetrieveSessionReturnsUnauthorizedIfPayloadExtractionFails(ExtractUserFromTokenErrors error)
        {
            var mockResult = CreateMockResultForExtractUser(error);

            JwtService.ExtractUserFromTokenAsync("a")
                .Returns(mockResult);

            var result = await AuthenticationController.RetrieveSession("a");
            var response = result as UnauthorizedResult;
            Assert.That(response?.StatusCode, Is.EqualTo(401));
        }

        [Test]
        public async Task Test_RetrieveSessionReturnsUserClaimsWhenSuccessful()
        {
            var mockResult = CreateMockResultForExtractUser(ExampleUser);

            JwtService.ExtractUserFromTokenAsync("a")
                .Returns(mockResult);

            var result = await AuthenticationController.RetrieveSession("a");
            var response = result as CreatedResult;

            var value = response?.Value;

            Assert.That(value, Is.EqualTo(ExampleUser));
        }

        [Test]
        [TestCase(FailedLoginError.WrongPassword)]
        [TestCase(FailedLoginError.UserDoesNotExist)]
        public async Task Test_LoginReturnsUnauthorizedWhenItFails(FailedLoginError error)
        {
            var login = ExampleLogin;
            var mockResult = CreateMockResultForLogin(error);

            AuthService.LoginAsync(login)
                .Returns(mockResult);

            var result = await AuthenticationController.Login(login);

            var response = result as UnauthorizedObjectResult;

            Assert.That(response?.StatusCode, Is.EqualTo(401));

            var responseError = response.Value as HttpError;
            Assert.That(responseError?.ErrorCode, Is.EqualTo(FailedLoginErrorCodes[error]));
        }

        [Test]
        public async Task Test_LoginReturnsASessionObjectWhenSuccessful()
        {
            var login = ExampleLogin;
            string token = "a";
            var mockResult = CreateMockResultForLogin(ExampleUser);

            AuthService.LoginAsync(login)
                .Returns(mockResult);

            JwtService.GenerateToken(ExampleUser)
                .Returns(Result<string, GenerateTokenErrors>
                    .Success(token));

            var result = await AuthenticationController.Login(login);

            var response = result as CreatedResult;
            Assert.That(response?.StatusCode, Is.EqualTo(201));

            var value = response.Value as SessionViewModel;
            Assert.Multiple(() =>
            {
                Assert.That(value?.Token, Is.EqualTo(token));
                Assert.That(value?.Id, Is.EqualTo(ExampleUser.Id));
            });
        }

        [Test]
        [TestCase(FailedRegisterError.Fail, 503)]
        [TestCase(FailedRegisterError.UsernameIsTaken, 400)]
        public async Task Test_RegisterReturnsCorrectErrorCodeUponFailure(FailedRegisterError error, int expectedStatusCode)
        {
            var register = ExampleRegister;
            var mockResult = CreateMockResultForRegister(error);

            AuthService.RegisterAsync(register)
                .Returns(mockResult);


            var result = await AuthenticationController.Register(register);
            var response = result as StatusCodeResult;
            Assert.That(response?.StatusCode, Is.EqualTo(expectedStatusCode));
        }

        [Test]
        public async Task Test_RegisterReturnsASessionOnSuccess()
        {
            var register = ExampleRegister;
            var mockResult = CreateMockResultForRegister(ExampleUser);
            string token = "a";

            AuthService.RegisterAsync(register)
                .Returns(mockResult);

            JwtService.GenerateToken(ExampleUser)
                .Returns(Result<string, GenerateTokenErrors>.Success(token));

            var result = await AuthenticationController.Register(register);

            var response = result as CreatedResult;
            var value = response?.Value as SessionViewModel;

            Assert.Multiple(() =>
            {
                Assert.That(value?.Id, Is.EqualTo(ExampleUser.Id));
                Assert.That(value?.Token, Is.EqualTo(token));
            });
        }

        [Test]
        [TestCase(true, 204)]
        [TestCase(false, 404)]
        public async Task Test_CheckIfUsernameIsTakenReturnsCorrectStatusCode(bool valueToBeReturned, int expectedStatusCode)
        {
            string username = "a";
            AuthService.CheckIfUsernameIsTakenAsync(username)
                .Returns(valueToBeReturned);

            var result = await AuthenticationController.CheckIfUsernameIsTaken(username);
            var response = result as StatusCodeResult;

            Assert.That(response?.StatusCode, Is.EqualTo(expectedStatusCode));
        }


        [Test]
        public async Task Test_LogoutReturnsNoContentIfSuccessful()
        {
            string jwt = "a";
            var mockResult = CreateMockResultForLogout(jwt);
            this.JwtStore.BlacklistTokenAsync(jwt)
                .Returns(mockResult);

            var result = await AuthenticationController.Logout(jwt);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        public async Task Test_LogoutReturnsForbiddenIfAlreadyBlacklistedErrorIsReturned()
        {
            string jwt = "a";
            var mockResult = CreateMockResultForLogout(BlacklistTokenError.AlreadyBlacklisted);
            this.JwtStore.BlacklistTokenAsync(jwt)
                .Returns(mockResult);

            var result = await AuthenticationController.Logout(jwt);
            Assert.That(result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        [TestCase(" ")]
        [TestCase("")]
        [TestCase(null)]
        public async Task Test_LogoutReturnsUnauthorizedIfJwtBearerIsWhitespaceOrNull(string? bearer)
        {
            var result = await AuthenticationController.Logout(bearer);
            Assert.That(result, Is.TypeOf<UnauthorizedResult>());
        }


        private static Result<UserViewModel, ExtractUserFromTokenErrors> CreateMockResultForExtractUser(UserViewModel user)
        {
            return Result<UserViewModel, ExtractUserFromTokenErrors>
                .Success(user);
        }

        private static Result<UserViewModel, ExtractUserFromTokenErrors> CreateMockResultForExtractUser(ExtractUserFromTokenErrors error)
        {
            return Result<UserViewModel, ExtractUserFromTokenErrors>
                .Failure(error);
        }

        private static Result<UserViewModel, FailedLoginError> CreateMockResultForLogin(UserViewModel user)
        {
            return Result<UserViewModel, FailedLoginError>
                .Success(user);
        }

        private static Result<UserViewModel, FailedLoginError> CreateMockResultForLogin(FailedLoginError error)
        {
            return Result<UserViewModel, FailedLoginError>
                .Failure(error);
        }

        private static Result<UserViewModel, FailedRegisterError> CreateMockResultForRegister(UserViewModel user)
        {
            return Result<UserViewModel, FailedRegisterError>
                .Success(user);
        }

        private static Result<UserViewModel, FailedRegisterError> CreateMockResultForRegister(FailedRegisterError error)
        {
            return Result<UserViewModel, FailedRegisterError>
                .Failure(error);
        }

        private static Result<string, BlacklistTokenError> CreateMockResultForLogout(BlacklistTokenError error)
        {
            return Result<string, BlacklistTokenError>
                .Failure(error);
        }

        private static Result<string, BlacklistTokenError> CreateMockResultForLogout(string value)
        {
            return Result<string, BlacklistTokenError>
                .Success(value);
        }
    }
}
