using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts;

namespace QuizWorld.Web
{
    /// <summary>
    /// Extends the ControllerBase. Authorizes every route by default. Allows easy access to the user's JWT
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// Returns the JWT attached to the Authorization header
        /// </summary>
        /// <returns>
        /// The token or an empty string if the Authorization header is not attached to the request
        /// </returns>
        public string GetJWT()
        {
            var headers = Request.Headers;
            if (!headers.ContainsKey("Authorization"))
            {
                return string.Empty;
            }

            headers.TryGetValue("Authorization", out var headerValues);
            string jwt = headerValues.First();
            return jwt;
        }
    }
}
