﻿using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Common.Constants.Sorting;
using QuizWorld.Common.Policy;
using QuizWorld.Common.Search;
using QuizWorld.Infrastructure.Legacy.ModelBinders.PaginationModelBinder;
using QuizWorld.Infrastructure.Legacy.ModelBinders.SortingOrderModelBinder;
using QuizWorld.Web.Contracts;
using QuizWorld.Web.Filters;

namespace QuizWorld.Web.Areas.Administration.Controllers
{
    [ApiController]
    [Route("/logs")]
    public class ActivityLogsController(IActivityLogger logger) : BaseController
    {
        private readonly IActivityLogger _logger = logger;

        [HttpGet]
        [ApiVersion("1.0")]
        [Obsolete("Append api-version query string with a value of the most recent version")]
        [Authorize(Policy = "CanAccessLogs", AuthenticationSchemes = "Bearer")]
        public async Task<ActionResult> GetLogsDeprecated(
            [ModelBinder(BinderType = typeof(PaginationModelBinder))] int page,
            [ModelBinder(BinderType = typeof(SortingOrderModelBinder))] SortingOrders order
            )
        {
            try
            {
                var result = await _logger.RetrieveLogs(page, order, 20);
                return Ok(result);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        [ApiVersion("2.0")]
        [NotFoundException]
        [Authorize(
            Policy = PolicyNames.CanViewLogs)]
        public async Task<IActionResult> GetLogs([FromQuery]SearchLogsParameters searchParameters)
        {
            var logs = await _logger.RetrieveLogs(searchParameters);
            return Ok(logs);
        }
    }
}
