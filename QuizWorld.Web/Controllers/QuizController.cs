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
    public class QuizController(IQuizService quizService) : BaseController
    {
        private readonly IQuizService _quizService = quizService;

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
        public async Task<IActionResult> Create(CreateQuizViewModel quiz)
        {
            string id = User!.FindFirst("id")!.Value;
            int createdQuizId = await _quizService.CreateAsync(
                quiz: quiz,
                userId: id,
                creationDate: DateTime.Now);

            CreatedResponseViewModel response = new()
            {
                Id = createdQuizId
            };

            return Created($"/quiz/{createdQuizId}", response);
        }
    }
}
