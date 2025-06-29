using Microsoft.AspNetCore.Authorization;

namespace QuizWorld.Infrastructure.AuthConfig.Requirements
{
    /// <summary>
    /// This policy can be used on any endpoint that exposes a "username" route parameter.
    /// If the user making the request does not have a JWT with that username, the request
    /// is blocked.
    /// </summary>
    public class JwtMatchesOwnUsernameRequirement : IAuthorizationRequirement
    {
    }
}
