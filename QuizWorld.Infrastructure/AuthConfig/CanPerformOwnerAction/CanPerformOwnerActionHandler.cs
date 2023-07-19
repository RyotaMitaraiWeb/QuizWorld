using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using QuizWorld.Infrastructure.Data.Entities;
using QuizWorld.ViewModels.Authentication;
using QuizWorld.Web.Contracts.JsonWebToken;
using QuizWorld.Web.Contracts.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.AuthConfig.CanPerformOwnerAction
{
    /// <summary>
    /// Checks if the user is the owner of the quiz or has at least one of the specified roles.
    /// </summary>
    public class CanPerformOwnerActionHandler : AuthorizationHandler<CanPerformOwnerActionRequirement>
    {
        private readonly IHttpContextAccessor http;
        private readonly IJwtService jwtService;
        private readonly IRepository repository;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IActivityLogger logger;
        public CanPerformOwnerActionHandler(
            IHttpContextAccessor http,
            IJwtService jwtService,
            IRepository repository,
            UserManager<ApplicationUser> userManager,
            IActivityLogger logger)
        {
            this.http = http;
            this.jwtService = jwtService;
            this.repository = repository;
            this.userManager = userManager;
            this.logger = logger;
        }
        /// <summary>
        /// Checks if the user is authorized to perform the requested action. A user is authorized
        /// if they are the creator of the quiz or have one of the specified roles.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requirement">The requirement which includes all roles that can perform the action alongside the creator</param>
        /// <returns></returns>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanPerformOwnerActionRequirement requirement)
        {
            var bearer = this.http.HttpContext?.Request.Headers.Authorization.FirstOrDefault();

            string jwt = this.jwtService.RemoveBearer(bearer);

            if (!await this.jwtService.CheckIfJWTIsValid(jwt))
            {
                await this.FailWithStatusCode(context, 401, "Invalid token");
                return;
            }

            UserViewModel user = this.jwtService.DecodeJWT(jwt);
            var quiz = await this.FindQuiz(context);
            if (quiz == null)
            {
                await this.FailWithStatusCode(context, 404, "Quiz does not exist");

                return;
            }


            if (user.Id == quiz.CreatorId.ToString())
            {
                context.Succeed(requirement);
                return;
            }
            else
            {
                try
                {
                    var applicationUser = await this.userManager.FindByIdAsync(user.Id);

                    if (applicationUser == null)
                    {
                        await this.FailWithStatusCode(context, 401, "Invalid token");
                        return;
                    }

                    var roles = await this.userManager.GetRolesAsync(applicationUser);
                    foreach (string role in roles)
                    {
                        if (requirement.RolesThatCanPerformAction.Contains(role))
                        {
                            string? route = this.http.HttpContext?.Request.GetEncodedPathAndQuery();
                            string? method = this.http.HttpContext?.Request.Method;
                            await this.logger.LogActivity(
                                $"{applicationUser.NormalizedUserName} sent a {method} request to {route}",
                                DateTime.Now);
                            context.Succeed(requirement);
                            return;
                        }
                    }
                }
                catch
                {
                    await this.FailWithStatusCode(context, 503, "Something went wrong with your request, try again later");
                    return;
                }
                

                context.Fail();
            }

        }

        /// <summary>
        /// Indicates to the context that authorization has failed and to return a response
        /// with the given status code and message.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="status"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private Task FailWithStatusCode(AuthorizationHandlerContext context, int status, string message)
        {
            context.Fail();
            this.http.HttpContext.Response.OnStarting(async () =>
            {
                this.http.HttpContext.Response.StatusCode = status;
                var responseText = Encoding.UTF8.GetBytes(message);
                await this.http.HttpContext.Response.Body.WriteAsync(responseText, 0, responseText.Length);
            });

            return Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves the quiz with the ID from the request.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>The quiz or null if it cannot retrieve the quiz</returns>
        private async Task<Quiz?> FindQuiz(AuthorizationHandlerContext context)
        {
            if (context.Resource is HttpContext httpContext)
            {
                var id = httpContext.GetRouteValue("id");
                if (id == null)
                {
                    return null;
                }

                var isInt = int.TryParse(id.ToString(), out int quizId);

                if (!isInt)
                {
                    return null;
                }

                var quiz = await this.repository.GetByIdAsync<Quiz>(quizId);
                if (quiz == null || quiz.IsDeleted)
                {
                    return null;
                }

                return quiz;
            }
            else
            {
                return null;
            }
        }
    }
}
