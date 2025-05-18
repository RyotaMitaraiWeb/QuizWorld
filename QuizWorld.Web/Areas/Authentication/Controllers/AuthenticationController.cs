using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Constants.InvalidActionsMessages;
using QuizWorld.Web.Contracts.JsonWebToken;
using QuizWorld.Infrastructure.Filters.GuestsOnly;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.ViewModels.Common;
using QuizWorld.Web.Contracts;
using static QuizWorld.Common.Constants.InvalidActionsMessages.InvalidActionsMessages;

namespace QuizWorld.Web.Areas.Authentication.Controllers
{
    [Route("/auth")]
    public class AuthenticationController : BaseController
    {
        private readonly IJwtService jwtService;
        private readonly IUserService userService;

        public AuthenticationController
            (
                IJwtService jwtService,
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
                var user = await this.userService.Register(register);
                if (user == null)
                {
                    var errors = new ErrorViewModel() { Errors = new string[] { "Something went wrong while registering you, please try again or check your input!" } };
                    return BadRequest(errors);
                }

                string jwt = this.jwtService.GenerateJWT(user);
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
                var errors = new ErrorViewModel() { Errors = new string[] { InvalidActionsMessages.RequestFailed } };
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
                var user = await this.userService.GetUser(id);
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
                var user = await this.userService.GetUserByUsername(username);
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
                var user = await this.userService.Login(login);
                if (user == null)
                {
                    var errors = new ErrorViewModel() { Errors = new string[] { InvalidActionsMessages.FailedLogin } };
                    return Unauthorized(errors);
                }

                string jwt = this.jwtService.GenerateJWT(user);
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
                var errors = new ErrorViewModel() { Errors = new string[] { InvalidActionsMessages.RequestFailed } };
                return StatusCode(503, errors);
            }
        }

        [HttpPost]
        [Route("session")]
        public async Task<IActionResult> Session([FromHeader(Name = "Authorization")] string? jwt)
        {
            string token = this.jwtService.RemoveBearer(jwt);
            try
            {
                var user = this.jwtService.DecodeJWT(token);
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
                var exists = await this.userService.CheckIfUsernameIsTaken(username);
                if (exists == false)
                {
                    return NotFound();
                }

                return Ok();
            }
            catch
            {
                var errors = new ErrorViewModel() { Errors = new string[] { InvalidActionsMessages.RequestFailed } };
                return StatusCode(503, errors);
            }
        }

        [HttpDelete]
        [Route("logout")]
        public async Task<IActionResult> Logout([FromHeader(Name = "Authorization")] string? jwt)
        {
            string token = this.jwtService.RemoveBearer(jwt);
            try
            {
                bool succeeded = await this.jwtService.InvalidateJWT(token);
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
