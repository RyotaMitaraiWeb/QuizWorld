using Asp.Versioning;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Web.Contracts.Authentication.JsonWebToken;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace QuizWorld.Web.Areas.Authentication.Controllers
{
    [Route("auth")]
    [ApiVersion("2.0")]
    public class AuthenticationController(IJwtService jwtService) : BaseController
    {
        private readonly IJwtService _jwtService = jwtService;

        [Route("session")]
        [HttpPost]
        public async Task<IActionResult> RetrieveSession([FromHeader(Name = "Authorization")] string? bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return Unauthorized();
            }

            var sessionValidationResult = await _jwtService.ExtractUserFromTokenAsync(bearerToken);
            if (sessionValidationResult.IsSuccess)
            {
                return Created("/auth/login",sessionValidationResult.Value);
            }

            return Unauthorized();
        }
    }
}
