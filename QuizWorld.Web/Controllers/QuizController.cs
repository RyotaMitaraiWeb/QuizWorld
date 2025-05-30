using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Errors;
using QuizWorld.Common.Http;
using QuizWorld.Common.Policy;
using QuizWorld.Common.Search;
using QuizWorld.ViewModels.Common;
using QuizWorld.ViewModels.Quiz;
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

        [Route("{id}")]
        [HttpPut]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = PolicyNames.CanEditAndDeleteAQuiz)]
        public async Task<IActionResult> EditQuizById(int id, EditQuizViewModel quiz)
        {
            await _quizService.EditAsync(id, quiz, DateTime.Now);
            return NoContent();
        }

        [Route("{id}")]
        [HttpDelete]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = PolicyNames.CanEditAndDeleteAQuiz)]
        public async Task<IActionResult> DeleteQuizById(int id)
        {
            await _quizService.DeleteAsync(id);
            return NoContent();
        }

        [Route("{id}/get-for-edit")]
        [HttpGet]
        [Authorize(
            AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme,
            Policy = PolicyNames.CanEditAndDeleteAQuiz)]
        public async Task<IActionResult> GetQuizForEdit(int id)
        {
            var quiz = await _quizService.GetForEditAsync(id);
            return Ok(quiz);
        }

        public static readonly string[] CanEditAndDeleteQuizzesEndpointMethods =
            [HttpMethod.Put.Method, HttpMethod.Delete.Method];

        public static readonly string GetQuizForEditEndpointEnding = "/get-for-edit";
    }
}
