using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using QuizWorld.Web.Contracts.JsonWebToken;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Areas.Authentication.Controllers;
using QuizWorld.Web.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using QuizWorld.Common.Constants.Roles;

namespace QuizWorld.Tests.Controllers.AuthenticationControllerUnitTests
{
    public class UnitTest
    {
        public Mock<IJwtService> jwtServiceMock { get; set; }
        public Mock<IUserService> userServiceMock { get; set; }
        public AuthenticationControllerV1 controller { get; set; }

        public UserViewModel user { get; set; }

        public RegisterViewModel register { get; set; }
        public LoginViewModel login { get; set; }

        [SetUp]
        public void Setup()
        {
            this.jwtServiceMock = new Mock<IJwtService>();
            this.userServiceMock = new Mock<IUserService>();
            this.controller = new AuthenticationControllerV1(jwtServiceMock.Object, userServiceMock.Object);
            this.user = new UserViewModel
            {
                Id = "a",
                Username = "ryota1",
                Roles = new string[] { Roles.User } 
            };

            this.register = new RegisterViewModel
            {
                Username = "ryota1",
                Password = "123456"
            };

            this.login = new LoginViewModel
            {
                Username = "ryota1",
                Password = "123456",
            };
        }

        [Test]
        public async Task Test_RegisterReturnsASessionViewModelWhenSuccessful()
        {
            this.userServiceMock
                .Setup(us => us.Register(this.register))
                .ReturnsAsync(this.user);

            this.jwtServiceMock
                .Setup(js => js.GenerateJWT(this.user))
                .Returns("a");

            var response = await this.controller.Register(this.register) as CreatedResult;

            var value = response.Value as SessionViewModel;
            Assert.Multiple(() =>
            {
                Assert.That(value.Token, Is.EqualTo("a"));
                Assert.That(value.Username, Is.EqualTo(this.register.Username));
                Assert.That(value.Roles, Has.Length.EqualTo(1));
                Assert.That(value.Roles[0], Is.EqualTo(Roles.User));
            });
        }

        [Test]
        public async Task Test_RegisterReturnsBadRequestIfUserRegistersUnsuccessfully()
        {
            this.userServiceMock
                .Setup(us => us.Register(this.register))
                .ReturnsAsync(() => null);

            var result = await this.controller.Register(this.register);
            
       

            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Test_RegisterReturnsServiceUnavailableIfUserServiceThrows()
        {
            this.userServiceMock
                .Setup(us => us.Register(this.register))
                .ThrowsAsync(new Exception());

            var response = await this.controller.Register(this.register);

            Assert.That(response, Is.TypeOf<ObjectResult>());
        }

        [Test]
        public async Task Test_LoginReturnsSessionIfUserLogsInSuccessfully()
        {
            this.userServiceMock
                .Setup(us => us.Login(this.login))
                .ReturnsAsync(this.user);

            this.jwtServiceMock
                .Setup(js => js.GenerateJWT(this.user))
                .Returns("a");


            var result = await this.controller.Login(this.login) as CreatedResult;
            var value = result.Value as SessionViewModel;


            Assert.Multiple(() =>
            {
                Assert.That(value.Token, Is.EqualTo("a"));
                Assert.That(value.Username, Is.EqualTo(this.register.Username));
                Assert.That(value.Roles, Has.Length.EqualTo(1));
                Assert.That(value.Roles[0], Is.EqualTo(Roles.User));
            });
        }

        [Test]
        public async Task Test_LoginReturnsUnauthorizedIfLoginFails()
        {
            this.userServiceMock
                .Setup(us => us.Login(this.login))
                .ReturnsAsync(() => null);


            var result = await this.controller.Login(this.login);
            Assert.That(result, Is.TypeOf<UnauthorizedObjectResult>());
        }

        [Test]
        public async Task Test_LoginReturnsServiceUnavailableIfUserServiceThrows()
        {
            this.userServiceMock
                .Setup(us => us.Login(this.login))
                .ThrowsAsync(new Exception());


            var result = await this.controller.Login(this.login);
            Assert.That(result, Is.TypeOf<ObjectResult>());
        }

        [Test]
        public async Task Test_SessionReturnsSessionViewModelIfTheUserHasAValidSession()
        {
            this.jwtServiceMock
                .Setup(js => js.DecodeJWT(It.IsAny<string>()))
                .Returns(this.user);

            this.jwtServiceMock
                .Setup(js => js.RemoveBearer(It.IsAny<string>()))
                .Returns("a");



            var result = await this.controller.Session("a") as CreatedResult;

            var value = result.Value as SessionViewModel;


            Assert.Multiple(() =>
            {
                Assert.That(value.Token, Is.EqualTo("a"));
                Assert.That(value.Username, Is.EqualTo(this.register.Username));
                Assert.That(value.Roles, Has.Length.EqualTo(1));
                Assert.That(value.Roles[0], Is.EqualTo(Roles.User));
            });
        }

        [Test]
        public async Task Test_SessionReturnsBadRequestIfJwtServiceThrows()
        {
            this.jwtServiceMock
                .Setup(js => js.DecodeJWT("a"))
                .Throws(new Exception());


            var result = await this.controller.Session("a");
            Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task Test_LogoutReturns204IfInvalidateTokenReturnsTrue()
        {
            this.jwtServiceMock
                .Setup(js => js.InvalidateJWT(It.IsAny<string>()))
                .ReturnsAsync(true);

            var result = await this.controller.Logout("a");
            

            Assert.That(result, Is.TypeOf<NoContentResult>());

        }

        [Test]
        public async Task Test_LogoutReturnsForbiddenIfInvalidateTokenReturnsFalse()
        {
            this.jwtServiceMock
                .Setup(js => js.InvalidateJWT("a"))
                .ReturnsAsync(false);

            var result = await this.controller.Logout("a");
           

            Assert.That(result, Is.TypeOf<ForbidResult>());

        }

        [Test]
        public async Task Test_LogoutReturnsObjectResultIfInvalidateTokenThrows()
        {
            this.jwtServiceMock
                .Setup(js => js.InvalidateJWT(It.IsAny<string>()))
                .ThrowsAsync(new Exception());

            var result = await this.controller.Logout("Bearer a");
            

            Assert.That(result, Is.TypeOf<ObjectResult>());
        }

        [Test]
        public async Task Test_UsernameExistsReturnsOkIfUserServiceReturnsTrue()
        {
            this.userServiceMock
                .Setup(us => us.CheckIfUsernameIsTaken("a"))
                .ReturnsAsync(true);

            var result = await this.controller.UsernameExists("a");
            

            Assert.That(result, Is.TypeOf<OkResult>());
        }

        [Test]
        public async Task Test_UsernameExistsReturns404IfUserServiceReturnsFalse()
        {
            this.userServiceMock
                .Setup(us => us.CheckIfUsernameIsTaken("a"))
                .ReturnsAsync(false);

            var result = await this.controller.UsernameExists("a");

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Test_UsernameExistsReturns503IfUserServiceThrows()
        {
            this.userServiceMock
                .Setup(us => us.CheckIfUsernameIsTaken("a"))
                .ThrowsAsync(new Exception());

            var result = await this.controller.UsernameExists("a");
            

            Assert.That(result, Is.TypeOf<ObjectResult>());
        }

        [Test]
        public async Task Test_GetProfileByUsernameReturnsOkIfUserServiceReturnsAUser()
        {
            this.userServiceMock
                .Setup(us => us.GetUserByUsername("a"))
                .ReturnsAsync(this.user);

            var result = await this.controller.GetProfileByUsername("a");


            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }

        [Test]
        public async Task Test_GetProfileByUsernameReturns404IfUserServiceReturnsNull()
        {
            this.userServiceMock
                .Setup(us => us.GetUserByUsername("a"))
                .ReturnsAsync(() => null);

            var result = await this.controller.GetProfileByUsername("a");

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Test_GetProfileByUsernameReturns503IfUserServiceThrows()
        {
            this.userServiceMock
                .Setup(us => us.GetUserByUsername("a"))
                .ThrowsAsync(new Exception());

            var result = await this.controller.GetProfileByUsername("a");


            Assert.That(result, Is.TypeOf<StatusCodeResult>());
        }
    }
}
