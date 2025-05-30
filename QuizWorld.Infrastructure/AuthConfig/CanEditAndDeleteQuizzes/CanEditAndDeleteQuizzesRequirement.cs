using Microsoft.AspNetCore.Authorization;

namespace QuizWorld.Infrastructure.AuthConfig.CanEditAndDeleteQuizzes
{
    public class CanEditAndDeleteQuizzesRequirement(params string[] rolesThatCanEditAndDeleteQuizzes) : IAuthorizationRequirement
    {
        public readonly string[] AllowedRoles = rolesThatCanEditAndDeleteQuizzes;
    }
}
