using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Http;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts.Authentication;
using QuizWorld.Web.Contracts.Authentication.JsonWebToken;
using System.Threading.Tasks;
using static QuizWorld.Common.Errors.AuthError;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace QuizWorld.Web.Areas.Authentication.Controllers
{
    [Route("auth")]
    [ApiVersion("2.0")]
    public class AuthenticationController(IJwtService jwtService, IAuthService authService) : BaseController
    {
        private readonly IJwtService _jwtService = jwtService;
        private readonly IAuthService _authService = authService;

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

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel credentials)
        {
            var result = await _authService.Login(credentials);
            if (result.IsFailure)
            {
                var error = new HttpError(FailedLoginErrorCodes[result.Error]);
                return Unauthorized(error);
            }

            SessionViewModel session = InitiateSession(result.Value);
            return Created(string.Empty, session);
        }

        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegisterViewModel registerBody)
        {
            var result = await _authService.Register(registerBody);
            if (result.IsFailure)
            {
                if (result.Error == FailedRegisterError.UsernameIsTaken)
                {
                    return BadRequest();
                }

                return StatusCode(503);
            }

            SessionViewModel session = InitiateSession(result.Value);
            return Created("", session);
        }

        [HttpGet]
        [Route("username")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckIfUsernameIsTaken([FromQuery] string username)
        {
            bool usernameIsTaken = await _authService.CheckIfUsernameIsTaken(username);
            if (usernameIsTaken)
            {
                return NoContent();
            }

            return NotFound();
        }

        private SessionViewModel InitiateSession(UserViewModel user)
        {
            var result = _jwtService.GenerateToken(user);
            if (result.IsFailure)
            {
                throw new Exception("Token generation failed");
            }

            SessionViewModel session = new()
            {
                Id = user.Id,
                Username = user.Username,
                Roles = user.Roles,
                Token = result.Value,
            };

            return session;
        }
    }
}
