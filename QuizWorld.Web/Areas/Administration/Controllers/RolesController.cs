using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Search;
using QuizWorld.ViewModels.Roles;
using QuizWorld.Web.Contracts.Roles;

namespace QuizWorld.Web.Areas.Administration.Controllers
{
    [Route("roles")]
    [ApiVersion("2.0")]
    public class RolesController(IRoleService roleService) : BaseController
    {
        private readonly IRoleService _roleService = roleService;

        [HttpGet]
        [Route("users")]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersParameters parameters)
        {
            var result = await _roleService.SearchUsers(parameters);
            return Ok(result);
        }
    }
}
