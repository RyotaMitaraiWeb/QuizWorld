﻿using QuizWorld.Common.Result;
using QuizWorld.ViewModels.Authentication;
using static QuizWorld.Common.Results.JwtError;

namespace QuizWorld.Web.Contracts
{
    public interface IJwtService
    {
        public Result<string, GenerateTokenErrors> GenerateToken(UserViewModel user);
        public Task<Result<UserViewModel, ExtractUserFromTokenErrors>> ExtractUserFromTokenAsync(string bearerToken);
    }
}
