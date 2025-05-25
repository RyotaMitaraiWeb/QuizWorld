using Asp.Versioning;
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
    [Route("quiz")]
    [ApiController]
    [ApiVersion("1.0")]
    [Obsolete("Deprecated")]
    public class QuizControllerV1 : BaseController
    {
        private readonly IQuizServiceDeprecated quizService;
        private readonly IJwtServiceDeprecated jwtService;
        public QuizControllerV1(IQuizServiceDeprecated quizService, IJwtServiceDeprecated jwtService)
        {
            this.quizService = quizService;
            this.jwtService = jwtService;
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("all")]
        public async Task<ActionResult> GetAll(
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingCategoryModelBinder))] SortingCategories category,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order)
        {
            try
            {
                var catalogue = await this.quizService.GetAllQuizzes(page, category, order, 6);
                return Ok(catalogue);
            }
            catch
            {
                return StatusCode(503);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("user/{id}")]
        public async Task<ActionResult> GetUserQuizzes(
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingCategoryModelBinder))] SortingCategories category,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order,
            string id)
        {
            try
            {
                var catalogue = await this.quizService.GetUserQuizzes(id, page, category, order, 6);
                return Ok(catalogue);
            }
            catch (ArgumentException)
            {
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(503);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("search")]
        public async Task<ActionResult> Search(
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingCategoryModelBinder))] SortingCategories category,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order,
            [FromQuery] string search)
        {
            try
            {
                var catalogue = await this.quizService.GetQuizzesByQuery(search, page, category, order, 6);
                return Ok(catalogue);
            }
            catch
            {
                return StatusCode(503);
            }
        }

        [HttpPost]
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

        [HttpGet]
        [AllowAnonymous]
        [Route("{id}")]
        public async Task<ActionResult> Get(int id)
        {
            try
            {
                var quiz = await this.quizService.GetQuizById(id);
                if (quiz == null)
                {
                    return NotFound();
                }

                return Ok(quiz);
            }
            catch
            {
                return StatusCode(503);
            }
        }

        [HttpDelete]
        [Authorize(Policy = "CanDeleteQuiz", AuthenticationSchemes = "Bearer")]
        [Route("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var result = await this.quizService.DeleteQuizById(id);
                if (result == null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch
            {
                return StatusCode(503);
            }
        }

        [HttpGet]
        [Authorize(Policy = "CanEditQuiz", AuthenticationSchemes = "Bearer")]
        [Route("{id}/edit")]
        public async Task<ActionResult> GetQuizForEdit(int id)
        {
            return Ok(await this.quizService.GetQuizForEdit(id));
        }

        [HttpPut]
        [Authorize(Policy = "CanEditQuiz", AuthenticationSchemes = "Bearer")]
        [Route("{id}")]
        public async Task<ActionResult> Edit([FromBody] EditQuizViewModel quiz, int id)
        {
            try
            {
                var result = await this.quizService.EditQuizById(id, quiz);
                if (result == null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch
            {
                return StatusCode(503);
            }
        }
    }
}
