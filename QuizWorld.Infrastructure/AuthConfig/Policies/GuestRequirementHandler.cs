using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using QuizWorld.Web.Contracts.JsonWebToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.AuthConfig.Policies
{
    public class GuestRequirementHandler : AuthorizationHandler<GuestRequirement>
    {
        private readonly IJwtService jwtService;
        private readonly IHttpContextAccessor http;
        public GuestRequirementHandler(IJwtService jwtService, IHttpContextAccessor http)
        {
            this.jwtService= jwtService;
            this.http = http;
        }
        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            GuestRequirement requirement
           )
        {
            string? bearer = this.http.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
            string token = this.jwtService.RemoveBearer(bearer);

            bool isValid = await this.jwtService.CheckIfJWTIsValid(token);

            if (!isValid )
            {
                context.Fail();
            }
            else
            {
                context.Succeed(requirement);
            }
        }
    }
}
