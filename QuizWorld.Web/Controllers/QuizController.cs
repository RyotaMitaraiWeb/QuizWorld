using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Errors;
using QuizWorld.Common.Http;
using QuizWorld.Common.Search;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.ViewModels.Common;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.Web.Contracts.Authentication.JsonWebToken;
using QuizWorld.Web.Contracts.Quiz;

namespace QuizWorld.Web.Controllers
{
    [Route("quiz")]
    [ApiVersion("2.0")]
    public class QuizController(IQuizService quizService, IJwtService jwtService) : BaseController
    {
        private readonly IQuizService _quizService = quizService;
        private readonly IJwtService _jwtService = jwtService;

        [Route("")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SearchQuizzes([FromQuery] QuizSearchParameterss searchParams)
        {
            var result = await _quizService.SearchAsync(searchParams);
            return Ok(result);
        }

        [Route("{id}")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _quizService.GetAsync(id);
            if (result.IsFailure)
            {
                var error = new HttpError(QuizError.QuizGetErrorCodes[result.Error]);
                return NotFound(error);
            }

            return Ok(result.Value);
        }

        [Route("")]
        [HttpPost]
        public async Task<IActionResult> Create(
            CreateQuizViewModel quiz,
            [FromHeader(Name = "Authorization")] string? bearerToken
            )
        {
            CreateOrEditQuizMetadata metadata = await ExtractQuizMetadata(bearerToken);

            int createdQuizId = await _quizService.CreateAsync(
                quiz: quiz,
                userId: metadata.User.Id,
                creationDate: metadata.Now);

            CreatedResponseViewModel response = new()
            {
                Id = createdQuizId
            };

            return Created($"/quiz/{createdQuizId}", response);

        }

        private async Task<CreateOrEditQuizMetadata> ExtractQuizMetadata(string? bearerToken)
        {
            var claimsResult = await _jwtService.ExtractUserFromTokenAsync(bearerToken ?? string.Empty);
            UserViewModel user = claimsResult.Value;
            var now = DateTime.Now;

            return new CreateOrEditQuizMetadata()
            {
                Now = now,
                User = user,
            };
        }
    }

    internal class CreateOrEditQuizMetadata
    {
        public DateTime Now { get; set; }
        public UserViewModel User { get; set; } = null!;
    }
}
