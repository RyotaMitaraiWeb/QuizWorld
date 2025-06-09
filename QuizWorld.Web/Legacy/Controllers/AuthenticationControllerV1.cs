using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Constants.InvalidActionsMessages;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.ViewModels.Common;
using static QuizWorld.Common.Constants.InvalidActionsMessages.InvalidActionsMessages;
using Asp.Versioning;
using QuizWorld.Web.Contracts.Legacy;
using QuizWorld.Infrastructure.Legacy.Filters.GuestsOnly;

namespace QuizWorld.Web.Legacy.Controllers
{
    [Route("auth")]
    [ApiVersion("1.0")]
    [Obsolete]
    public class AuthenticationControllerV1 : BaseController
    {
        private readonly IJwtServiceDeprecated jwtService;
        private readonly IUserService userService;

        public AuthenticationControllerV1
            (
                IJwtServiceDeprecated jwtService,
                IUserService userService
            )
        {
            this.jwtService = jwtService;
            this.userService = userService;
        }

        [HttpPost]
        [AllowAnonymous]
        [GuestsOnly]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel register)
        {
            try
            {
                var user = await userService.Register(register);
                if (user == null)
                {
                    var errors = new ErrorViewModel() { Errors = new string[] { "Something went wrong while registering you, please try again or check your input!" } };
                    return BadRequest(errors);
                }

                string jwt = jwtService.GenerateJWT(user);
                var session = new SessionViewModel()
                {
                    Id = user.Id,
                    Username = user.Username,
                    Roles = user.Roles,
                    Token = jwt
                };

                return Created("/users" + session.Username, session);
            }
            catch
            {
                var errors = new ErrorViewModel() { Errors = new string[] { RequestFailed } };
                return StatusCode(503, errors);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("{id}")]
        public async Task<IActionResult> Profile(string id)
        {
            try
            {
                var user = await userService.GetUser(id);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch
            {
                return StatusCode(503);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("get-by-username/{username}")]
        public async Task<IActionResult> GetProfileByUsername(string username)
        {
            try
            {
                var user = await userService.GetUserByUsername(username);
                if (user == null)
                {
                    return NotFound();
                }

                return Ok(user);
            }
            catch
            {
                return StatusCode(503);
            }
        }

        [AllowAnonymous]
        [GuestsOnly]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel login)
        {
            try
            {
                var user = await userService.Login(login);
                if (user == null)
                {
                    var errors = new ErrorViewModel() { Errors = new string[] { FailedLogin } };
                    return Unauthorized(errors);
                }

                string jwt = jwtService.GenerateJWT(user);
                var session = new SessionViewModel()
                {
                    Id = user.Id,
                    Username = user.Username,
                    Roles = user.Roles,
                    Token = jwt
                };


                return Created("/users" + session.Username, session);
            }
            
            catch
            {
                var errors = new ErrorViewModel() { Errors = new string[] { RequestFailed } };
                return StatusCode(503, errors);
            }
        }

        [HttpPost]
        [Route("session")]
        public async Task<IActionResult> Session([FromHeader(Name = "Authorization")] string? jwt)
        {
            string token = jwtService.RemoveBearer(jwt);
            try
            {
                var user = jwtService.DecodeJWT(token);
                var session = new SessionViewModel()
                {
                    Token = token,
                    Id = user.Id,
                    Username = user.Username,
                    Roles = user.Roles,
                };


                return Created($"/users/{user.Username}", session);
            }
            catch
            {
                var errors = new ErrorViewModel() { Errors = new string[] { "Malformed JWT" } };
                return BadRequest(errors);
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("username/{username}")]
        public async Task<IActionResult> UsernameExists(string username)
        {
            try
            {
                var exists = await userService.CheckIfUsernameIsTaken(username);
                if (exists == false)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch
            {
                var errors = new ErrorViewModel() { Errors = new string[] { RequestFailed } };
                return StatusCode(503, errors);
            }
        }

        [HttpDelete]
        [Route("logout")]
        public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string? jwt)
        {
            string token = jwtService.RemoveBearer(jwt);
            try
            {
                bool succeeded = await jwtService.InvalidateJWT(token);
                if (!succeeded)
                {
                    return Forbid("Bearer");
                }

                return NoContent();
            }

            catch (Exception e)
            {
                
                var errors = new ErrorViewModel() { Errors = new string[] { e.Message } };
                return StatusCode(503, errors);
            }
        }
    }
}
