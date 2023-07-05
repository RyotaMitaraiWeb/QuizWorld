using Microsoft.Extensions.Configuration;
using Moq;
using QuizWorld.Infrastructure.Data.Contracts;
using QuizWorld.Infrastructure.Data.Services.JsonWebToken;
using QuizWorld.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Services.JwtServiceUnitTests
{
    public class UnitTest
    {
        public Mock<IJwtBlacklist> blacklistMock { get; set; }
        public Mock<IConfiguration> configMock { get; set; }
        public JwtService service { get; set; }

        [SetUp]
        public void Setup()
        {
            this.blacklistMock = new Mock<IJwtBlacklist>();
            this.configMock = new Mock<IConfiguration>();
            this.service = new JwtService(this.blacklistMock.Object, this.configMock.Object);
        }

        [Test]
        public async Task Test_CheckIfJWTHasBeenInvalidatedReturnsTrueIfTheBlacklistFindsTheToken()
        {
            this.blacklistMock
                .Setup(b => b.FindJWT(It.IsAny<string>()))
                .ReturnsAsync("a");

            var result = await this.service.CheckIfJWTHasBeenInvalidated("a");
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Test_CheckIfJWTHasBeenInvalidatedReturnsFalseIfTheBlacklistDoesNotFindTheToken()
        {
            this.blacklistMock
                .Setup(b => b.FindJWT(It.IsAny<string>()))
                .ReturnsAsync(() => null);

            var result = await this.service.CheckIfJWTHasBeenInvalidated("a");
            Assert.That(result, Is.False);
        }

        [Test]
        public void Test_DecodeJWTSuccessfullyReturnsAUser()
        {
            this.configMock
                .SetupGet(c => c["JWT:Secret"])
                .Returns("aswenwe12tasgq3qwsas3t");


            this.configMock
                .SetupGet(c => c["JWT:ValidIssuer"])
                .Returns("localhost:5000");

            this.configMock
                .SetupGet(c => c["JWT:ValidAudience"])
                .Returns("localhost:4200");

            var user = new UserViewModel()
            {
                Id = "aaaaa",
                Username = "ryota1",
                Roles = new string[] { "User" }
            };

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

        [Test]
        public void Test_GenerateJWTGeneratesAToken()
        {
            this.configMock
                .SetupGet(c => c["JWT:Secret"])
                .Returns("aswenwe12tasgq3qwsas3t");


            this.configMock
                .SetupGet(c => c["JWT:ValidIssuer"])
                .Returns("localhost:5000");

            this.configMock
                .SetupGet(c => c["JWT:ValidAudience"])
                .Returns("localhost:4200");

            var user = new UserViewModel()
            {
                Id = "aaaaa",
                Username = "ryota1",
                Roles = new string[] { "User" }
            };

            var token = this.service.GenerateJWT(user);
            Assert.That(token, Has.Length.GreaterThan(20));
        }

        [Test]
        public async Task Test_InvalidateTokenReturnsTrueIfActionIsSuccessful()
        {
            this.blacklistMock
                .Setup(b => b.BlacklistJWT("a"))
                .ReturnsAsync(true);

            var result = await this.service.InvalidateJWT("a");
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Test_InvalidateTokenReturnsFalseIfActionIsUnsuccessful()
        {
            this.blacklistMock
                .Setup(b => b.BlacklistJWT("a"))
                .ReturnsAsync(false);

            var result = await this.service.InvalidateJWT("a");
            Assert.That(result, Is.False);
        }
    }
}
