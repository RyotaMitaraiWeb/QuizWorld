using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts.JsonWebToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.AuthConfig.CanAccessLogs
{
    /// <summary>
    /// Checks whether the user has a role that can access logs. For security purposes, all unauthorized
    /// requests are responded with 404.
    /// </summary>
    public class CanAccessLogsHandler : AuthorizationHandler<CanAccessLogsRequirement>
    {
        private readonly IHttpContextAccessor http;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJwtService jwtService;
        private readonly ILogger<CanAccessLogsHandler> logger;

        public CanAccessLogsHandler(
            IHttpContextAccessor http,
            UserManager<ApplicationUser> userManager,
            IJwtService jwtService,
            ILogger<CanAccessLogsHandler> logger)
        {
            this.http = http;
            this.userManager = userManager;
            this.jwtService = jwtService;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAccessLogsRequirement requirement)
        {
            var bearer = this.http.HttpContext?.Request.Headers.Authorization.FirstOrDefault();

            string jwt = this.jwtService.RemoveBearer(bearer);

            this.logger.LogInformation("POLICY ADMIN");

            if (!await this.jwtService.CheckIfJWTIsValid(jwt))
            {
                await this.Fail(context);
                return;
            }

            UserViewModel user = this.jwtService.DecodeJWT(jwt);
            string id = user.Id;

            var applicationUser = await this.userManager.FindByIdAsync(id);
            var userRoles = await this.userManager.GetRolesAsync(applicationUser);
            
            foreach (var role in userRoles )
            {
                if (requirement.RolesThatCanAccessLogs.Contains(role))
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            await this.Fail(context);
            return;
        }

        /// <summary>
        /// Indicates to the context that authorization has failed and to return a response
        /// with the given status code and message.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private Task Fail(AuthorizationHandlerContext context)
        {
            context.Fail();
            this.http.HttpContext.Response.OnStarting(async () =>
            {
                this.http.HttpContext.Response.StatusCode = 404;
                var responseText = Encoding.UTF8.GetBytes("Page does not exist");
                await this.http.HttpContext.Response.Body.WriteAsync(responseText, 0, responseText.Length);
            });

            return Task.CompletedTask;
        }
    }
}
