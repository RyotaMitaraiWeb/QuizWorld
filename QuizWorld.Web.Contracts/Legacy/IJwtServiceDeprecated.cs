using QuizWorld.ViewModels.Authentication;

namespace QuizWorld.Web.Contracts.Legacy;

[Obsolete("Deprecated as part of the refactor")]
public interface IJwtServiceDeprecated
{
    public string GenerateJWT(UserViewModel user);
    public UserViewModel DecodeJWT(string jwt);
    public Task<bool> InvalidateJWT(string jwt);
    public Task<bool> CheckIfJWTHasBeenInvalidated(string jwt);
    public string RemoveBearer(string? bearerToken);
    public Task<bool> CheckIfJWTIsValid(string jwt);
}
