using QuizWorld.Common.Result;
using QuizWorld.ViewModels.Authentication;
using static QuizWorld.Common.Errors.AuthError;

namespace QuizWorld.Web.Contracts
{
    public interface IAuthService
    {
        Task<Result<UserViewModel, FailedLoginError>> LoginAsync(LoginViewModel credentials);
        Task<Result<UserViewModel, FailedRegisterError>> RegisterAsync(RegisterViewModel body);
        Task<bool> CheckIfUsernameIsTakenAsync(string username);
    }
}
