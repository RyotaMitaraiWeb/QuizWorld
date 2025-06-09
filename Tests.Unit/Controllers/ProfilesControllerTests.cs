using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts;
using QuizWorld.Web.Controllers;

namespace Tests.Unit.Controllers
{
    public class ProfilesControllerTests
    {
        public IProfileService ProfileService { get; set; }
        public ProfilesController ProfilesController { get; set; }

        [SetUp]
        public void Setup()
        {
            ProfileService = Substitute.For<IProfileService>();
            ProfilesController = new ProfilesController(ProfileService);
        }

        [Test]
        public async Task Test_GetProfileByUsernameReturnsNotFoundIfAProfileCannotBeFound()
        {
            UserViewModel? user = null;
            string username = "a";

            ProfileService
                .GetUserByUsername(username)
                .Returns(user);

            var result = await ProfilesController.GetProfileByUsername(username);

            Assert.That(result, Is.TypeOf<NotFoundResult>());
        }

        [Test]
        public async Task Test_GetProfileByUsernameReturnsOKfAProfileIsFound()
        {
            UserViewModel? user = new UserViewModel();
            string username = "a";

            ProfileService
                .GetUserByUsername(username)
                .Returns(user);

            var result = await ProfilesController.GetProfileByUsername(username);

            Assert.That(result, Is.TypeOf<OkObjectResult>());
        }
    }
}
