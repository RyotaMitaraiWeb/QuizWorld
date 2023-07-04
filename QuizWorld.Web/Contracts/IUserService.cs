using QuizWorld.ViewModels.Authentication;

namespace QuizWorld.Web.Contracts
{
    public interface IUserService
    {
        public Task<UserViewModel?> Register(RegisterViewModel user);
        public Task<UserViewModel?> Login(LoginViewModel user);
    }
}
