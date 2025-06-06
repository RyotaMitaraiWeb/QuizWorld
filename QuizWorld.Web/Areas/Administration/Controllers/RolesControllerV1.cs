using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Infrastructure.ModelBinders;
using QuizWorld.ViewModels.Common;
using QuizWorld.Web.Contracts.Roles;

namespace QuizWorld.Web.Areas.Administration.Controllers
{
    [ApiController]
    [Route("/roles")]
    [ApiVersion("1.0")]
    [Obsolete]
    public class RolesControllerV1 : BaseController
    {
        private readonly IRoleServiceDeprecated roleService;

        public RolesControllerV1(IRoleServiceDeprecated roleService)
        {
            this.roleService = roleService;
        }

        [HttpGet]
        [Route("users")]
        [Authorize(Policy = "CanSeeRoles", AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> GetUsersByUsername(
            [FromQuery] string username,
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order
            )
        {
            try
            {
                var users = await this.roleService.GetUsersByUsername(username, page, order);
                return Ok(users);
            }
            catch
            {
                return StatusCode(503);
            }
        }

        [HttpGet]
        [Route("users/{role}")]
        [Authorize(Policy = "CanSeeRoles", AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetUsersOfRole(
            string role,
            [FromQuery] string? username,
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order
        )
        {
            try
            {
                var users = await this.roleService.GetUsersOfRole(role, username ?? string.Empty, page, order, 20);
                return Ok(users);

            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch
            {
                return StatusCode(503);
            }
        }

        [HttpPut]
        [Route("promote/{userId}/{role}")]
        [Authorize(Policy = "CanChangeRoles", AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GiveUserRole(
            string userId,
            string role,
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order)
        {
            try
            {
                var result = await this.roleService.GiveUserRole(userId, role);
                if (result == null)
                {
                    return BadRequest();
                }

                var updatedUserList = await this.roleService.GetUsersOfRole(role, page, order, 20);
                return Ok(updatedUserList);
            }
            catch (ArgumentException e)
            {
                var errors = new ErrorViewModel() { Errors = new string[] { e.Message } };
                return NotFound(errors);
            }
            catch (Exception e)
            {
                return StatusCode(503);
            }
        }

        [HttpPut]
        [Route("demote/{userId}/{role}")]
        [Authorize(Policy = "CanChangeRoles", AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> RemoveRoleFromUser(
            string userId,
            string role,
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order)
        {
            try
            {
                var result = await this.roleService.RemoveRoleFromUser(userId, role);
                if (result == null)
                {
                    return BadRequest();
                }

                var updatedUserList = await this.roleService.GetUsersOfRole(role, page, order, 20);
                return Ok(updatedUserList);
            }
            catch (ArgumentException e)
            {
                var errors = new ErrorViewModel() { Errors = new string[] { e.Message } };
                return NotFound(errors);
            }
            catch
            {
                return StatusCode(503);
            }
        }
    }
}
