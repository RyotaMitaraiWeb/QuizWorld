using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using QuizWorld.Common.Hubs;
using QuizWorld.Common.Policy;
using QuizWorld.ViewModels.Roles;
using QuizWorld.Web.Contracts;
using QuizWorld.Web.Filters;
using QuizWorld.Web.Hubs;
using static QuizWorld.Common.Errors.RoleError;

namespace QuizWorld.Web.Areas.Administration.Controllers
{
    [Route("roles")]
    [ApiVersion("2.0")]
    [Authorize(Policy = PolicyNames.CanInteractWithRoles)]
    public class RolesController(IRoleService roleService, IHubContext<SessionHub> sessionHubContext) : BaseController
    {
        private readonly IRoleService _roleService = roleService;
        private readonly IHubContext<SessionHub> _sessionHubContext = sessionHubContext;

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

            var notification = CreateNotification(model);

            await _sessionHubContext.Clients
                .Group(GroupNames.UserIdGroup(model.UserId))
                .SendAsync(HubMethodNames.Session.RoleAdded, notification);
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

            var notification = CreateNotification(model);

            await _sessionHubContext.Clients
                .Group(GroupNames.UserIdGroup(model.UserId))
                .SendAsync(HubMethodNames.Session.RoleRemoved, notification);

            return NoContent();
        }

        private static RoleChangeNotificationViewModel CreateNotification(ChangeRoleViewModel model) 
            => new() { Role = model.Role };
    }
}
