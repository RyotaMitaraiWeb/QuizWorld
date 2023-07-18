using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Web.Contracts.Quiz;

namespace QuizWorld.Web.Controllers
{
    [Route("/grade")]
    [ApiController]
    [AllowAnonymous]
    public class GradeController : BaseController
    {
        private readonly IGradeService gradeService;

        public GradeController(IGradeService gradeService)
        {
            this.gradeService = gradeService;
        }

        [Route("{id}/question")]
        [HttpGet]
        public async Task<ActionResult> GetCorrectAnswersForQuestion(string id, [FromQuery] int version)
        {
            try
            {
                var result = await this.gradeService.GetCorrectAnswersForQuestionById(id, version);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch
            {
                return StatusCode(503);
            }
        }

        [Route("{id}/quiz")]
        [HttpGet]
        public async Task<ActionResult> GetCorrectAnswersForQuiz(int id, [FromQuery] int version)
        {
            try
            {
                var result = await this.gradeService.GetCorrectAnswersForQuestionsByQuizId(id, version);
                if (result == null)
                {
                    return NotFound();
                }

                return Ok(result);
            }
            catch (InvalidOperationException)
            {
                return BadRequest();
            }
            catch
            {
                return StatusCode(503);
            }
        }
    }
}
