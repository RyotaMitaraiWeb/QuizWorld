using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Question;
using QuizWorld.ViewModels.Quiz;

namespace QuizWorld.Web.Controllers
{
    [Route("/quiz")]
    [ApiController]
    public class QuizController : BaseController
    {
        [HttpPost]
        [AllowAnonymous]
        [Route("create")]
        public CreateQuizViewModel Create([FromBody]CreateQuizViewModel quiz)
        {
            return quiz;
        }
    }
}
