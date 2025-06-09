using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Policy;
using QuizWorld.ViewModels.Roles;
using QuizWorld.Web.Contracts;
using QuizWorld.Web.Filters;
using static QuizWorld.Common.Errors.RoleError;

namespace QuizWorld.Web.Areas.Administration.Controllers
{
    [Route("roles")]
    [ApiVersion("2.0")]
    [Authorize(Policy = PolicyNames.CanInteractWithRoles)]
    public class RolesController(IRoleService roleService) : BaseController
    {
        private readonly IRoleService _roleService = roleService;

        [HttpPatch("add")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(LogRoleChangeFilter))]
        public async Task<IActionResult> AddRoleToUser(ChangeRoleViewModel model)
        {
            var result = await _roleService.GiveUserRole(model);
            if (result.IsFailure)
            {
                if (result.Error == AddRoleError.UserDoesNotExist)
                {
                    return NotFound();
                }

                return BadRequest();
            }

            return NoContent();
        }

        [HttpPatch("remove")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ServiceFilter(typeof(LogRoleChangeFilter))]
        public async Task<IActionResult> RemoveRoleFromUser(ChangeRoleViewModel model)
        {
            var result = await _roleService.RemoveRoleFromUser(model);
            if (result.IsFailure)
            {
                if (result.Error == RemoveRoleError.UserDoesNotExist)
                {
                    return NotFound();
                }

                return BadRequest();
            }

            return NoContent();
        }
    }
}
