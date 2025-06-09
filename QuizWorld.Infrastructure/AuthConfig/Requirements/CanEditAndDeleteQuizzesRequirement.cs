using Microsoft.AspNetCore.Authorization;

namespace QuizWorld.Infrastructure.AuthConfig.Requirements
{
    public class CanEditAndDeleteQuizzesRequirement(params string[] rolesThatCanEditAndDeleteQuizzes) : IAuthorizationRequirement
    {
        public readonly string[] AllowedRoles = rolesThatCanEditAndDeleteQuizzes;
    }
}
