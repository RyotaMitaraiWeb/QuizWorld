using QuizWorld.ViewModels.Authentication;

namespace QuizWorld.Web.Contracts
{
    public interface ISessionHubClient
    {
        Task ReceiveCredentials(UserViewModel user);
    }
}
