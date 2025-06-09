using QuizWorld.Common.Search;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.ViewModels.UserList;

namespace QuizWorld.Web.Contracts
{
    public interface IProfileService
    {
        Task<UserViewModel?> GetUserByUsername(string username);
        Task<ListUsersViewModel> SearchUsers(SearchUsersParameters parameters);
    }
}
