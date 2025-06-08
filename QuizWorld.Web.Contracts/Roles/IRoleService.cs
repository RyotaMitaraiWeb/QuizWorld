using QuizWorld.Common.Result;
using QuizWorld.Common.Search;
using QuizWorld.ViewModels.Roles;
using QuizWorld.ViewModels.UserList;
using static QuizWorld.Common.Errors.RoleError;

namespace QuizWorld.Web.Contracts.Roles
{
    public interface IRoleService
    {
        Task<ListUsersViewModel> SearchUsers(SearchUsersParameters parameters);
        Task<Result<string, AddRoleError>> GiveUserRole(ChangeRoleViewModel data);
        Task<Result<string, RemoveRoleError>> RemoveRoleFromUser(ChangeRoleViewModel data);
    }
}
