using QuizWorld.Common.Result;
using QuizWorld.ViewModels.Authentication;
using static QuizWorld.Common.Errors.AuthError;

namespace QuizWorld.Web.Contracts.Authentication
{
    public interface IAuthService
    {
        Task<Result<UserViewModel, FailedLoginError>> Login(LoginViewModel credentials);
        Task<Result<UserViewModel, FailedRegisterError>> Register(RegisterViewModel body);
        Task<bool> CheckIfUsernameIsTaken(string username);
    }
}
