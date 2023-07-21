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
    public class RolesController : BaseController
    {
        private readonly IRoleService roleService;

        public RolesController(IRoleService roleService)
        {
            this.roleService = roleService;
        }

        [HttpGet]
        [Route("users/{role}")]
        [Authorize(Policy = "CanSeeRoles", AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetUsersOfRole(
            string role,
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order
        )
        {
            try
            {
                var users = await this.roleService.GetUsersOfRole(role, page, order, 20);
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
        [Route("promote/{userid}/{role}")]
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
            catch
            {
                return StatusCode(503);
            }
        }

        [HttpPut]
        [Route("demote/{userid}/{role}")]
        [Authorize(Policy = "CanChangeRoles", AuthenticationSchemes = "Bearer")]
        public Task<ActionResult> RemoveRoleFromUser(
            string userId,
            string role,
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order)
        {
            throw new NotImplementedException();
        }
    }
}
