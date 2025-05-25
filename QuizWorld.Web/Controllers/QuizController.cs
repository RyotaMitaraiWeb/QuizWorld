using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Search;
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
            var result = await _quizService.Search(searchParams);
            return Ok(result);
        }
    }
}
