using Moq;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Services.Legacy;
using QuizWorld.Web.Contracts.Legacy;

namespace QuizWorld.Tests.Services.UserServiceTest
{
    public class InMemory
    {
        public TestDB testDb;
        public UserService service;
        public Mock<IJwtServiceDeprecated> jwtServiceMock;

        [SetUp]
        public async Task Setup()
        {
            this.jwtServiceMock= new Mock<IJwtServiceDeprecated>();
            this.testDb = new TestDB();
            this.service = new UserService(testDb.userManager, this.jwtServiceMock.Object);
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

        [Test]
        public async Task Test_CheckIfUsernameIsTakenReturnsTrueIfAUserWithTheUsernameExistsInTheDatabase()
        {
            var result = await this.service.CheckIfUsernameIsTaken(this.testDb.User.UserName);
            Assert.That(result, Is.True);
        }

        [Test]
        public async Task Test_CheckIfUsernameIsTakenReturnsFalseIfAUserWithTheUsernameDoesNotExistInTheDatabase()
        {
            var result = await this.service.CheckIfUsernameIsTaken("!");
            Assert.That(result, Is.False);
        }
    }
}
