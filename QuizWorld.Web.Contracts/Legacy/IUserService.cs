using QuizWorld.ViewModels.Authentication;

namespace QuizWorld.Web.Contracts.Legacy
{
    [Obsolete("Deprecated. Use IAuthService")]
    public interface IUserService
    {
        public Task<UserViewModel?> Register(RegisterViewModel user);
        public Task<UserViewModel?> Login(LoginViewModel user);
        public Task<bool> Logout(string jwt);
        public Task<bool> CheckIfUsernameIsTaken(string username);
        public Task<UserViewModel?> GetUser(string id);
        public Task<UserViewModel?> GetUserByUsername(string username);
    }
}
