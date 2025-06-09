using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Web.Contracts.Legacy;

namespace QuizWorld.Web.Legacy.Controllers
{
    [Route("/grade")]
    [ApiController]
    [AllowAnonymous]
    [ApiVersion("1.0")]
    [Obsolete]
    public class GradeControllerV1 : BaseController
    {
        private readonly IGradeServiceDeprecated gradeService;

        public GradeControllerV1(IGradeServiceDeprecated gradeService)
        {
            this.gradeService = gradeService;
        }

        [Route("{id}/question")]
        [HttpGet]
        public async Task<ActionResult> GetCorrectAnswersForQuestion(string id, [FromQuery] int version)
        {
            try
            {
                var result = await gradeService.GetCorrectAnswersForQuestionById(id, version);
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
                var result = await gradeService.GetCorrectAnswersForQuestionsByQuizId(id, version);
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
