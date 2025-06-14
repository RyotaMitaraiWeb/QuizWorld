﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts.Legacy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.AuthConfig.Legacy.CanAccessLogs
{
    [Obsolete]
    /// <summary>
    /// Checks whether the user has a role that can access logs. For security purposes, all unauthorized
    /// requests are responded with 404.
    /// </summary>
    public class CanAccessLogsHandler : AuthorizationHandler<CanAccessLogsRequirement>
    {
        private readonly IHttpContextAccessor http;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IJwtServiceDeprecated jwtService;
        private readonly ILogger<CanAccessLogsHandler> logger;

        public CanAccessLogsHandler(
            IHttpContextAccessor http,
            UserManager<ApplicationUser> userManager,
            IJwtServiceDeprecated jwtService,
            ILogger<CanAccessLogsHandler> logger)
        {
            this.http = http;
            this.userManager = userManager;
            this.jwtService = jwtService;
            this.logger = logger;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAccessLogsRequirement requirement)
        {
            var bearer = http.HttpContext?.Request.Headers.Authorization.FirstOrDefault();

            string jwt = jwtService.RemoveBearer(bearer);

            logger.LogInformation("POLICY ADMIN");

            if (!await jwtService.CheckIfJWTIsValid(jwt))
            {
                await Fail(context);
                return;
            }

            UserViewModel user = jwtService.DecodeJWT(jwt);
            string id = user.Id;

            var applicationUser = await userManager.FindByIdAsync(id);
            var userRoles = await userManager.GetRolesAsync(applicationUser);
            
            foreach (var role in userRoles )
            {
                if (requirement.RolesThatCanAccessLogs.Contains(role))
                {
                    context.Succeed(requirement);
                    return;
                }
            }

            await Fail(context);
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
            http.HttpContext.Response.OnStarting(async () =>
            {
                http.HttpContext.Response.StatusCode = 404;
                var responseText = Encoding.UTF8.GetBytes("Page does not exist");
                await http.HttpContext.Response.Body.WriteAsync(responseText, 0, responseText.Length);
            });

            return Task.CompletedTask;
        }
    }
}
