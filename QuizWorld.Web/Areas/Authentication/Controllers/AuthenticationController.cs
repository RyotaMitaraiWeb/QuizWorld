using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Http;
using QuizWorld.Common.Util;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts;
using static QuizWorld.Common.Errors.AuthError;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace QuizWorld.Web.Areas.Authentication.Controllers
{
    [Route("auth")]
    [ApiVersion("2.0")]
    public class AuthenticationController(IJwtService jwtService, IAuthService authService, IJwtStore jwtStore) : BaseController
    {
        private readonly IJwtService _jwtService = jwtService;
        private readonly IAuthService _authService = authService;
        private readonly IJwtStore _jwtStore = jwtStore;

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
            var result = await _authService.LoginAsync(credentials);
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
            var result = await _authService.RegisterAsync(registerBody);
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
            bool usernameIsTaken = await _authService.CheckIfUsernameIsTakenAsync(username);
            if (usernameIsTaken)
            {
                return NoContent();
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("logout")]
        public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string? bearerToken)
        {
            if (string.IsNullOrWhiteSpace(bearerToken))
            {
                return Unauthorized();
            }

            string jwt = JwtUtil.RemoveBearer(bearerToken);

            var result = await _jwtStore.BlacklistTokenAsync(jwt);
            if (result.IsSuccess)
            {
                return NoContent();
            }

            return Forbid();
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
