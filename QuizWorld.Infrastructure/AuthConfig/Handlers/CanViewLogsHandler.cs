using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using QuizWorld.Common.Claims;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Infrastructure.AuthConfig.Requirements;
using QuizWorld.Infrastructure.Data.Entities.Identity;

namespace QuizWorld.Infrastructure.AuthConfig.Handlers
{
    public class CanViewLogsHandler(UserManager<ApplicationUser> userManager)
        : AuthorizationHandler<HasRolesRequirement>
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasRolesRequirement requirement)
        {
            var user = await GetUser(context);
            var roles = await GetRoles(user);
            
            bool hasRequiredRoles = Roles.HasRequiredRoles(roles, requirement.RequiredRoles);
            if (!hasRequiredRoles)
            {
                context.Fail();
                return;
            }

            context.Succeed(requirement);
        }

        private async Task<ApplicationUser?> GetUser(AuthorizationHandlerContext context)
        {
            string? userId = context.User.FindFirst(UserClaimsProperties.Id)?.Value;
            if (userId is null) return null;

            ApplicationUser? user = await _userManager.FindByIdAsync(userId);
            return user;
        }

        private async Task<string[]> GetRoles(ApplicationUser? user)
        {
            if (user is null) return [];

            var roles = await _userManager.GetRolesAsync(user);
            return [.. roles];
        }

        public const string Name = "CanViewLogsPolicy";
    }
}
