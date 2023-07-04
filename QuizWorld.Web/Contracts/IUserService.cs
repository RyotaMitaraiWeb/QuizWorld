using QuizWorld.ViewModels.Authentication;

namespace QuizWorld.Web.Contracts
{
    public interface IUserService
    {
        public Task<UserViewModel?> Register(RegisterViewModel user);
        public Task<UserViewModel?> Login(LoginViewModel user);
        public string GenerateJWT(UserViewModel user);
        public UserViewModel DecodeJWT(string jwt);
        public Task<bool> Logout(string jwt);
    }
}
