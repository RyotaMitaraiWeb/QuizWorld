using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Search;
using QuizWorld.Infrastructure.AuthConfig.Handlers;
using QuizWorld.ViewModels.Profile;
using QuizWorld.Web.Contracts;

namespace QuizWorld.Web.Controllers
{
    [Route("profile")]
    public class ProfilesController(IProfileService profileService) : BaseController
    {
        private readonly IProfileService _profileService = profileService;

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> SearchProfiles([FromQuery] SearchUsersParameters parameters)
        {
            var result = await _profileService.SearchUsers(parameters);
            return Ok(result);
        }

        [HttpGet("username/{username}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetProfileByUsername(string username)
        {
            var result = await _profileService.GetUserByUsername(username);
            if (result is null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPatch("{username}/profile-picture")]
        [Authorize(Policy = JwtMatchesOwnUsernameHandler.Name)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdateProfilePicture(UploadProfilePictureViewModel model, string username)
        {
            var result = await _profileService.UploadProfilePicture(model, username);
            return Ok(result);
        }

        [HttpDelete("{username}/profile-picture")]
        [Authorize(Policy = JwtMatchesOwnUsernameHandler.Name)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteProfilePicture(string username)
        {
            var metadata = new DeleteProfilePictureViewModel()
            { 
                Username = username
            };

            var result = await _profileService.DeleteProfilePicture(metadata);

            if (!result.ProfilePictureExisted)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
