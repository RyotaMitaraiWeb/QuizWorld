using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Infrastructure.ModelBinders;
using QuizWorld.ViewModels.Answer;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.ViewModels.Common;
using QuizWorld.ViewModels.Question;
using QuizWorld.ViewModels.Quiz;
using QuizWorld.Web.Contracts.JsonWebToken;
using QuizWorld.Web.Contracts.Quiz;

namespace QuizWorld.Web.Controllers
{
    [Route("/quiz")]
    [ApiController]
    public class QuizController : BaseController
    {
        private readonly IQuizService quizService;
        private readonly IJwtService jwtService;
        public QuizController(IQuizService quizService, IJwtService jwtService)
        {
            this.quizService = quizService;
            this.jwtService = jwtService;
        }


        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody] CreateQuizViewModel quiz, [FromHeader(Name = "Authorization")] string? token)
        {
            try
            {
                string jwt = this.jwtService.RemoveBearer(token);
                UserViewModel user = this.jwtService.DecodeJWT(jwt);

                int id = await this.quizService.CreateQuiz(quiz, user.Id);
                var response = new CreatedResponseViewModel() { Id = id };

                return Created("/quiz/" + id, response);
            }
            catch (InvalidOperationException)
            {
                return Unauthorized();
            }
            catch
            {
                return StatusCode(503);
            }
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[Route("create")]
        //public CreateQuizViewModel Create([FromBody] CreateQuizViewModel quiz)
        //{
        //    return quiz;
        //}

        [HttpGet]
        [AllowAnonymous]
        [Route("page")]
        public object Page([ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order)
        {

            return order;
        }
    }
}
