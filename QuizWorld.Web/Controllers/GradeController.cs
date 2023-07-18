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
        public Task<ActionResult> GetCorrectAnswersForQuestion(string id, [FromQuery] int version)
        {
            throw new NotImplementedException();
        }

        [Route("{id}/quiz")]
        [HttpGet]
        public Task<ActionResult> GetCorrectAnswersForQuiz(int id, [FromQuery] int version)
        {
            throw new NotImplementedException();
        }
    }
}
