using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NSubstitute;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Result;
using QuizWorld.ViewModels.Roles;
using QuizWorld.Web.Areas.Administration.Controllers;
using QuizWorld.Web.Contracts;
using QuizWorld.Web.Hubs;
using static QuizWorld.Common.Errors.RoleError;

namespace Tests.Unit.Controllers
{
    public class RolesControllerTests
    {
        public IRoleService RoleService { get; set; }
        public RolesController RolesController { get; set; }
        public ChangeRoleViewModel ChangeRoleViewModel { get; set; }
        public IHubContext<SessionHub> HubContext { get; set; }

        [SetUp]
        public void Setup()
        {
            RoleService = Substitute.For<IRoleService>();
            HubContext = Substitute.For<IHubContext<SessionHub>>();
            RolesController = new RolesController(RoleService, HubContext);
            ChangeRoleViewModel = new()
            {
                UserId = Guid.NewGuid().ToString(),
                Role = Roles.Moderator,
            };
        }

        [Test]
        public async Task Test_AddRoleToUserReturnsNoContentWhenSuccessful()
        {
            var mockResult = Result<string, AddRoleError>.Success("a");

            RoleService
                .GiveUserRole(ChangeRoleViewModel)
                .Returns(mockResult);

            var result = await RolesController.AddRoleToUser(ChangeRoleViewModel);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [TestCase(AddRoleError.UserDoesNotExist, 404)]
        [TestCase(AddRoleError.UserAlreadyHasRole, 400)]
        public async Task Test_AddRoleToUserReturnsCorrectStatusCodeWhenAnErrorIsReturnedFromService(AddRoleError error, int statusCode)
        {
            var mockResult = Result<string, AddRoleError>.Failure(error);

            RoleService
                .GiveUserRole(ChangeRoleViewModel)
                .Returns(mockResult);

            var result = await RolesController.AddRoleToUser(ChangeRoleViewModel);
            var response = result as StatusCodeResult;

            Assert.That(response?.StatusCode, Is.EqualTo(statusCode));
        }

        [Test]
        public async Task Test_RemoveRoleFromUserReturnsNoContentWhenSuccessful()
        {
            var mockResult = Result<string, RemoveRoleError>.Success("a");

            RoleService
                .RemoveRoleFromUser(ChangeRoleViewModel)
                .Returns(mockResult);

            var result = await RolesController.RemoveRoleFromUser(ChangeRoleViewModel);
            Assert.That(result, Is.TypeOf<NoContentResult>());
        }

        [Test]
        [TestCase(RemoveRoleError.UserDoesNotExist, 404)]
        [TestCase(RemoveRoleError.UserDoesNotHaveRoleInFirstPlace, 400)]
        public async Task Test_RemoveRoleFromUserReturnsCorrectStatusCodeWhenAnErrorIsReturnedFromService(RemoveRoleError error, int statusCode)
        {
            var mockResult = Result<string, RemoveRoleError>.Failure(error);

            RoleService
                .RemoveRoleFromUser(ChangeRoleViewModel)
                .Returns(mockResult);

            var result = await RolesController.RemoveRoleFromUser(ChangeRoleViewModel);
            var response = result as StatusCodeResult;

            Assert.That(response?.StatusCode, Is.EqualTo(statusCode));
        }
    }
}
