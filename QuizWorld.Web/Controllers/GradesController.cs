using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Web.Contracts;
using static QuizWorld.Common.Errors.GradeError;

namespace QuizWorld.Web.Controllers
{
    [Route("grade")]
    [ApiVersion("2.0")]
    public class GradesController(IGradeService gradeService) : BaseController
    {
        private readonly IGradeService _gradeService = gradeService;

        [HttpGet("question/{questionId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GradeQuestion(string questionId, [FromQuery] int version)
        {
            var result = await _gradeService.GradeQuestion(questionId, version);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result.Error == GradeQuestionError.QuestionOrVersionDoesNotExist)
            {
                return NotFound();
            }

            return BadRequest();
        }

        [HttpGet("quiz/{quizId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [AllowAnonymous]
        public async Task<IActionResult> GradeQuiz(int quizId, [FromQuery] int version)
        {
            var result = await _gradeService.GradeQuiz(quizId, version);
            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            if (result.Error == GradeEntireQuizError.QuizOrVersionDoesNotExist)
            {
                return NotFound();
            }

            return BadRequest();
        }
    }
}
