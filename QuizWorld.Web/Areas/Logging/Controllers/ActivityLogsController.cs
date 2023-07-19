using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Infrastructure.ModelBinders;
using QuizWorld.Web.Contracts.Logging;

namespace QuizWorld.Web.Areas.Logging.Controllers
{
    [ApiController]
    [Route("/logs")]
    public class ActivityLogsController : BaseController
    {
        private readonly IActivityLogger logger;
        public ActivityLogsController(IActivityLogger logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        [Authorize(Policy = "CanAccessLogs", AuthenticationSchemes = "Bearer")]
        public Task<ActionResult> GetLogs(
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order
            )
        {
            throw new NotImplementedException();
        }
    }
}
