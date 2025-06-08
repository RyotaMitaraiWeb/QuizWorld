using Microsoft.AspNetCore.Authorization;

namespace QuizWorld.Infrastructure.AuthConfig.Requirements
{
    public class HasRolesRequirement(params string[] roles) : IAuthorizationRequirement
    {
        public readonly string[] RequiredRoles = roles;
    }
}
