using Microsoft.AspNetCore.Mvc;
using MockQueryable.Moq;
using Moq;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.ViewModels.UserList;
using QuizWorld.Web.Areas.Administration.Controllers;
using QuizWorld.Web.Contracts.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Controllers.RolesControllerUnitTests
{
    public class UnitTest
    {
        public Mock<IRoleService> roleServiceMock;
        public RolesController controller;

        [SetUp]
        public void Setup()
        {
            this.roleServiceMock = new Mock<IRoleService>();
            this.controller = new RolesController(this.roleServiceMock.Object);
        }

        [Test]
        public async Task Test_GetUsersOfRoleReturnsOkWithAListOfUsers()
        {
            var list = this.GenerateUserList();

            this.roleServiceMock
                .Setup(rs => rs.GetUsersOfRole(Roles.Moderator, 1, SortingOrders.Ascending, 20))
                .ReturnsAsync(new ListUsersViewModel() { Total = 3, Users = list });

            var result = await this.controller.GetUsersOfRole(Roles.Moderator, 1, SortingOrders.Ascending) as OkObjectResult;
            var value = result.Value as ListUsersViewModel;
            Assert.That(value.Users, Is.EqualTo(list));
        }

        [Test]
        public async Task Test_GetUsersOfRoleReturnsBadRequestIfRoleServiceThrowsArgumentException()
        {
            var list = this.GenerateUserList();

            this.roleServiceMock
                .Setup(rs => rs.GetUsersOfRole("Janitor", 1, SortingOrders.Ascending, 20))
                .ThrowsAsync(new ArgumentException());

            var result = await this.controller.GetUsersOfRole("Janitor", 1, SortingOrders.Ascending);
            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task Test_GetUsersOfRoleReturnsServiceUnavailableIfRoleServiceThrowsAGenericException()
        {
            var list = this.GenerateUserList();

            this.roleServiceMock
                .Setup(rs => rs.GetUsersOfRole(Roles.Moderator, 1, SortingOrders.Ascending, 20))
                .ThrowsAsync(new Exception());

            var result = await this.controller.GetUsersOfRole(Roles.Moderator, 1, SortingOrders.Ascending);
            Assert.That(result, Is.TypeOf<StatusCodeResult>());
        }

        [Test]
        public async Task Test_GiveUserRoleReturnsOkWithAListOfUsersIfRoleServiceChangesUserRole()
        {
            var id = Guid.NewGuid();
            var list = this.GenerateUserList();
            this.roleServiceMock
                .Setup(rs => rs.GiveUserRole(id.ToString(), Roles.Moderator))
                .ReturnsAsync(id);

            this.roleServiceMock
                .Setup(rs => rs.GetUsersOfRole(Roles.Moderator, 1, SortingOrders.Ascending, 20))
                .ReturnsAsync(new ListUsersViewModel() { Total = 3, Users = list });

            var result = await this.controller.GiveUserRole(id.ToString(), Roles.Moderator, 1, SortingOrders.Ascending) as OkObjectResult;
            var value = result.Value as ListUsersViewModel;
            Assert.That(value.Users, Is.EqualTo(list));
        }

        [Test]
        public async Task Test_GiveUserRoleReturnsNotFoundIfRolesServiceThrowsArgumentException()
        {
            var id = Guid.NewGuid();
            var list = this.GenerateUserList();
            this.roleServiceMock
                .Setup(rs => rs.GiveUserRole(id.ToString(), Roles.Moderator))
                .ThrowsAsync(new ArgumentException("error"));

            

            var result = await this.controller.GiveUserRole(id.ToString(), Roles.Moderator, 1, SortingOrders.Ascending);
            
            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Test_GiveUserRoleReturnsBadRequestIfRolesServiceReturnsNull()
        {
            var id = Guid.NewGuid();
            var list = this.GenerateUserList();
            this.roleServiceMock
                .Setup(rs => rs.GiveUserRole(id.ToString(), Roles.Moderator))
                .ReturnsAsync(() => null);



            var result = await this.controller.GiveUserRole(id.ToString(), Roles.Moderator, 1, SortingOrders.Ascending);

            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task Test_GiveUserRoleReturnsServiceUnavailableIfRolesServiceThrowsAGenericException()
        {
            var id = Guid.NewGuid();
            var list = this.GenerateUserList();
            this.roleServiceMock
                .Setup(rs => rs.GiveUserRole(id.ToString(), Roles.Moderator))
                .ThrowsAsync(new Exception());



            var result = await this.controller.GiveUserRole(id.ToString(), Roles.Moderator, 1, SortingOrders.Ascending);

            Assert.That(result, Is.TypeOf<StatusCodeResult>());
        }

        [Test]
        public async Task Test_RemoveRoleFromUserReturnsOkWithAListOfUsersIfRoleServiceChangesUserRole()
        {
            var id = Guid.NewGuid();
            var list = this.GenerateUserList();
            this.roleServiceMock
                .Setup(rs => rs.RemoveRoleFromUser(id.ToString(), Roles.Moderator))
                .ReturnsAsync(id);

            this.roleServiceMock
                .Setup(rs => rs.GetUsersOfRole(Roles.Moderator, 1, SortingOrders.Ascending, 20))
                .ReturnsAsync(new ListUsersViewModel() { Total = 3, Users = list });

            var result = await this.controller.RemoveRoleFromUser(id.ToString(), Roles.Moderator, 1, SortingOrders.Ascending) as OkObjectResult;
            var value = result.Value as ListUsersViewModel;
            Assert.That(value.Users, Is.EqualTo(list));
        }

        [Test]
        public async Task Test_RemoveRoleFromUserReturnsNotFoundIfRolesServiceThrowsArgumentException()
        {
            var id = Guid.NewGuid();
            var list = this.GenerateUserList();
            this.roleServiceMock
                .Setup(rs => rs.RemoveRoleFromUser(id.ToString(), Roles.Moderator))
                .ThrowsAsync(new ArgumentException("error"));



            var result = await this.controller.RemoveRoleFromUser(id.ToString(), Roles.Moderator, 1, SortingOrders.Ascending);

            Assert.That(result, Is.TypeOf<NotFoundObjectResult>());
        }

        [Test]
        public async Task Test_RemoveRoleFromUserReturnsBadRequestIfRolesServiceReturnsNull()
        {
            var id = Guid.NewGuid();
            var list = this.GenerateUserList();
            this.roleServiceMock
                .Setup(rs => rs.RemoveRoleFromUser(id.ToString(), Roles.Moderator))
                .ReturnsAsync(() => null);



            var result = await this.controller.RemoveRoleFromUser(id.ToString(), Roles.Moderator, 1, SortingOrders.Ascending);

            Assert.That(result, Is.TypeOf<BadRequestResult>());
        }

        [Test]
        public async Task Test_RemoveRoleFromUserReturnsServiceUnavailableIfRolesServiceThrowsAGenericException()
        {
            var id = Guid.NewGuid();
            var list = this.GenerateUserList();
            this.roleServiceMock
                .Setup(rs => rs.RemoveRoleFromUser(id.ToString(), Roles.Moderator))
                .ThrowsAsync(new Exception());



            var result = await this.controller.RemoveRoleFromUser(id.ToString(), Roles.Moderator, 1, SortingOrders.Ascending);

            Assert.That(result, Is.TypeOf<StatusCodeResult>());
        }

        [Test]
        public async Task Test_GetUsersByUsernameReturnsOkWithAListOfUsers()
        {
            var list = this.GenerateUserList();

            this.roleServiceMock
                .Setup(rs => rs.GetUsersByUsername("e", 1, SortingOrders.Ascending, 20))
                .ReturnsAsync(new ListUsersViewModel() { Total = 3, Users = list });

            var result = await this.controller.GetUsersByUsername("e", 1, SortingOrders.Ascending) as OkObjectResult;
            var value = result.Value as ListUsersViewModel;
            Assert.That(value.Users, Is.EqualTo(list));
        }

        [Test]
        public async Task Test_GetUsersByUsernameReturnsServiceUnavailableIfRoleServiceThrowsAnException()
        {
            var list = this.GenerateUserList();

            this.roleServiceMock
                .Setup(rs => rs.GetUsersByUsername("e", 1, SortingOrders.Ascending, 20))
                .ThrowsAsync(new Exception());

            var result = await this.controller.GetUsersByUsername("e", 1, SortingOrders.Ascending);
            Assert.That(result, Is.TypeOf<StatusCodeResult>());
        }

        private List<ListUserItemViewModel> GenerateUserList()
        {
            return new List<ListUserItemViewModel>()
            {
                new ListUserItemViewModel()
                {
                    Id = "1",
                    Roles = new List<string>() { Roles.Admin, Roles.Moderator },
                    Username = "admin",
                },
                new ListUserItemViewModel()
                {
                    Id = "2",
                    Roles = new List<string>() { Roles.Moderator },
                    Username = "moderator",
                },
            };
        }
    }
}