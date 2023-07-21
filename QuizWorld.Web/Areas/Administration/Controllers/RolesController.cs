using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Infrastructure.ModelBinders;
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
        public Task<ActionResult> GetUsersOfRole(
            string role,
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order
        )
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        [Route("promote/{userid}/{role}")]
        [Authorize(Policy = "CanChangeRoles", AuthenticationSchemes = "Bearer")]
        public Task<ActionResult> GiveUserRole(
            string userId,
            string role,
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order)
        {
            throw new NotImplementedException();
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
