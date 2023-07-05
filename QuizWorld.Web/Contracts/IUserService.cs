using QuizWorld.ViewModels.Authentication;

namespace QuizWorld.Web.Contracts
{
    public interface IUserService
    {
        public Task<UserViewModel?> Register(RegisterViewModel user);
        public Task<UserViewModel?> Login(LoginViewModel user);
        public Task<bool> Logout(string jwt);
        public Task<bool> CheckIfUsernameIsTaken(string username);
    }
}
