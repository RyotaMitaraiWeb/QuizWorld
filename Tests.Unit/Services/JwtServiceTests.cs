using Castle.Core.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Areas.Logging.Controllers;
using QuizWorld.Web.Contracts.Authentication.JsonWebToken;
using QuizWorld.Web.Contracts.Logging;
using QuizWorld.Web.Services.Authentication.JsonWebToken.JwtService;
using System.IdentityModel.Tokens.Jwt;
using static QuizWorld.Common.Results.JwtError;

namespace QuizWorld.Tests.Unit.Services
{
    public class JwtServiceTests
    {
        public JwtService JwtService { get; set; }
        public IConfiguration Config { get; set; }
        public UserViewModel ExampleUser = new ()
        {
            Id = Guid.NewGuid().ToString(),
            Username = "admin",
            Roles = [Roles.Admin, Roles.Moderator, Roles.User],
        };

        [SetUp]
        public void Setup()
        {
            Config = Substitute.For<IConfiguration>();
            JwtService = new JwtService(Config);
        }

        [Test]
        public void Test_GenerateTokenAsyncReturnsATokenWhenSuccessful()
        {
            Config["JWT_SECRET"] = "aswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3t";

            var result = JwtService.GenerateToken(ExampleUser);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.True);
                Assert.That(result.Value, Has.Length.GreaterThan(20));
            });
        }

        [Test]
        public void Test_GenerateTokenReturnsErrorWhenUnsuccessful()
        {
            Config["JWT_SECRET"] = "";

            var result = JwtService.GenerateToken(ExampleUser);

            Assert.Multiple(() =>
            {
                Assert.That(result.IsSuccess, Is.False);
                Assert.That(result.Error, Is.EqualTo(GenerateTokenErrors.Fail));
            });
        }
    }
}
