using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using QuizWorld.Common.Claims;
using QuizWorld.Common.Hubs;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts;

namespace QuizWorld.Web.Hubs
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SessionHub(UserManager<ApplicationUser> userManager) : Hub<ISessionHubClient>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        public override async Task OnConnectedAsync()
        {
            string? userId = GetUserId();
            if (userId is null)
            {
                await base.OnConnectedAsync();
                return;
            }

            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            if (user is null)
            {
                await base.OnConnectedAsync();
                return;
            }

            string group = GroupNames.UserIdGroup(userId);

            await Groups.AddToGroupAsync(Context.ConnectionId, group);
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new UserViewModel()
            {
                Id = userId,
                Username = user.UserName!,
                Roles = [.. roles],
            };

            await Clients.Groups(group).ReceiveCredentials(claims);
            await base.OnConnectedAsync();
        }

        private string? GetUserId()
        {
            string? userId = Context?.User?.FindFirst(UserClaimsProperties.Id)?.Value;
            return userId;

        }
    }
}
