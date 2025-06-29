﻿using Microsoft.Extensions.Configuration;
using Moq;
using QuizWorld.ViewModels.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuizWorld.Web.Services.Legacy;
using QuizWorld.Web.Contracts.Legacy;

namespace QuizWorld.Tests.Services.JwtServiceUnitTests
{
    public class UnitTest
    {
        public Mock<IJwtBlacklistDeprecated> blacklistMock { get; set; }
        public Mock<IConfiguration> configMock { get; set; }
        public JwtServiceDeprecated service { get; set; }

        [SetUp]
        public void Setup()
        {
            this.blacklistMock = new Mock<IJwtBlacklistDeprecated>();
            this.configMock = new Mock<IConfiguration>();
            this.service = new JwtServiceDeprecated(this.blacklistMock.Object, this.configMock.Object);
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
                .SetupGet(c => c["JWT_SECRET"])
                .Returns("aswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3t");


            this.configMock
                .SetupGet(c => c["JWT_VALID_ISSUER"])
                .Returns("localhost:5000");

            this.configMock
                .SetupGet(c => c["JWT_VALID_AUDIENCE"])
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
                .SetupGet(c => c["JWT_SECRET"])
                .Returns("aswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3t");


            this.configMock
                .SetupGet(c => c["JWT_VALID_ISSUER"])
                .Returns("localhost:5000");

            this.configMock
                .SetupGet(c => c["JWT_VALID_AUDIENCE"])
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

        [Test]
        public void Test_RemoveBearerRemovesTheBearerTextFromAToken()
        {
            string token = this.service.RemoveBearer("Bearer test");
            Assert.That(token, Is.EqualTo("test"));
        }

        [Test]
        public void Test_RemoveBearerReturnsAnEmptyStringIfNullIsPassed()
        {
            string token = this.service.RemoveBearer(null);
            Assert.That(token, Is.EqualTo(string.Empty));
        }

        [Test]
        [TestCase("test", ExpectedResult = "test")]
        [TestCase("BearerTest", ExpectedResult = "BearerTest")]
        [TestCase("Bearer", ExpectedResult = "Bearer")]
        public string Test_RemoveBearerReturnsTheSameStringIfItDoesContainBearerWithSpace(string token)
        {

            return this.service.RemoveBearer(token);
        }

        [Test]
        public async Task Test_CheckIfJWTIsValidReturnsTrueIfTokenIsNotFoundInTheBlacklist()
        {
            var user = new UserViewModel()
            {
                Id = "aaaaa",
                Username = "ryota1",
                Roles = new string[] { "User" }
            };

            this.configMock
                .SetupGet(c => c["JWT_SECRET"])
                .Returns("aswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3t");

            this.configMock
                .SetupGet(c => c["JWT_VALID_ISSUER"])
                .Returns("localhost:5000");

            this.configMock
                .SetupGet(c => c["JWT_VALID_AUDIENCE"])
                .Returns("localhost:4200");

            string token = this.service.GenerateJWT(user);

            this.blacklistMock
                .Setup(b => b.FindJWT(token))
                .ReturnsAsync(() => null);

            bool result = await this.service.CheckIfJWTIsValid(token);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Test_CheckIfJWTIsValidReturnsFalseIfTokenIsFoundInTheBlacklist()
        {
            var user = new UserViewModel()
            {
                Id = "aaaaa",
                Username = "ryota1",
                Roles = new string[] { "User" }
            };

            this.configMock
                .SetupGet(c => c["JWT_SECRET"])
                .Returns("aswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3t");

            this.configMock
                .SetupGet(c => c["JWT_VALID_ISSUER"])
                .Returns("localhost:5000");

            this.configMock
                .SetupGet(c => c["JWT_VALID_AUDIENCE"])
                .Returns("localhost:4200");

            string token = this.service.GenerateJWT(user);

            this.blacklistMock
                .Setup(b => b.FindJWT(token))
                .ReturnsAsync(token);

            bool result = await this.service.CheckIfJWTIsValid(token);
            Assert.That(result, Is.False);
        }

        [Test]
        public async Task Test_CheckIfJWTIsValidReturnsFalseIfTokenValidationFails()
        {
            var user = new UserViewModel()
            {
                Id = "aaaaa",
                Username = "ryota1",
                Roles = new string[] { "User" }
            };

            // Act as if the client tampered with the JWT and thus changed its secret
            this.configMock
                .SetupGet(c => c["JWT_SECRET"])
                .Returns("aswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3taswenwe12tasgq3qwsas3t");

            this.configMock
                .SetupGet(c => c["JWT_VALID_ISSUER"])
                .Returns("localhost:5000");

            this.configMock
                .SetupGet(c => c["JWT_VALID_AUDIENCE"])
                .Returns("localhost:4200");

            string token = this.service.GenerateJWT(user);

            this.configMock
                .SetupGet(c => c["JWT_SECRET"])
                .Returns("correctsecret");

            this.blacklistMock
                .Setup(b => b.FindJWT(token))
                .ReturnsAsync(token);

            bool result = await this.service.CheckIfJWTIsValid(token);
            Assert.That(result, Is.False);
        }
    }
}
