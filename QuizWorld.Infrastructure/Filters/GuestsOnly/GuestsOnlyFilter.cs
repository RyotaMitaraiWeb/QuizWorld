using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using QuizWorld.Web.Contracts.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Filters.GuestsOnly
{
    /// <summary>
    /// Prevents access to a given route or controller by logged in users. Only guests can
    /// access it.
    /// </summary>
    public class GuestsOnlyFilter : IAsyncAuthorizationFilter
    {
        private readonly IJwtServiceDeprecated jwtService;
        private readonly IHttpContextAccessor http;
        public GuestsOnlyFilter(IJwtServiceDeprecated jwtService, IHttpContextAccessor http)
        {
            this.jwtService = jwtService;
            this.http = http;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            try
            {
                string? bearer = http.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
                string token = jwtService.RemoveBearer(bearer);

                bool isValid = await jwtService.CheckIfJWTIsValid(token);
                if (isValid)
                {
                    context.Result = new ForbidResult("Bearer");
                    return;
                }
            }
            catch
            {
                context.Result = new StatusCodeResult(503);
                return;
            }
        }
    }
}
