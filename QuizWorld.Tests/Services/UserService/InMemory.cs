using Microsoft.Extensions.Configuration;
using Moq;
using QuizWorld.Infrastructure;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Services.UserServiceTest
{
    public class InMemory
    {
        public TestDB testDb;
        public UserService service;

        [SetUp]
        public async Task Setup()
        {
            var config = new Mock<IConfiguration>();
            this.testDb = new TestDB();
            this.service = new UserService(testDb.userManager, config.Object);
        }

        [Test]
        public async Task Test_RegisterSuccessfullyAddsAUserWithTheCorrectProperties()
        {
            var register = new RegisterViewModel()
            {
                Username = "ryota2",
                Password = "123456",
            };

            var result = await this.service.Register(register);
            var user = await this.testDb.userManager.FindByNameAsync(register.Username);
            Assert.Multiple(() =>
            {
                Assert.That(user, Is.Not.Null);
                Assert.That(user.UserName, Is.EqualTo(register.Username));
            });
        }

        [Test]
        public async Task Test_RegisterReturnsNullIfInputIsInvalid()
        {
            // The username is valid, but password isn't
            var register = new RegisterViewModel()
            {
                Username = "ryota2",
                Password = "1",
            };

            var result = await this.service.Register(register);
            var user = await this.testDb.userManager.FindByNameAsync(register.Username);
            Assert.That(user, Is.Null);
        }
      
        [Test]
        public async Task Test_LoginReturnsAUserOnSuccessfulLogin()
        {
            var login = new LoginViewModel()
            {
                Username = this.testDb.User.UserName,
                Password = "123456"
            };

            var result = await this.service.Login(login);
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result.Username, Is.EqualTo(login.Username));
            });
        }

        [Test]
        public async Task Test_LoginReturnsNullOnWrongPassword()
        {
            var login = new LoginViewModel()
            {
                Username = this.testDb.User.UserName,
                Password = "123456123623ds"
            };

            var result = await this.service.Login(login);
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task Test_LoginReturnsNullOnNonExistantUsername()
        {
            var login = new LoginViewModel()
            {
                Username = "aaaaaa",
                Password = "123456"
            };

            var result = await this.service.Login(login);
            Assert.That(result, Is.Null);
        }
    }
}
