using Microsoft.AspNetCore.Authentication.JwtBearer;
using QuizWorld.Infrastructure.Data.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.AuthConfig
{
    /// <summary>
    /// Configures the JWT middleware to refer to the JWT blacklist when validating the token
    /// </summary>
    public class AppJwtBearerEvents : JwtBearerEvents
    {
        private readonly IJwtService jwtService;
        public AppJwtBearerEvents(IJwtService jwtService)
        {
            this.jwtService = jwtService;
        }

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            string? bearer = context.Request.Headers.Authorization.FirstOrDefault();
            string token = this.jwtService.RemoveBearer(bearer);

            bool isBlacklisted = await this.jwtService.CheckIfJWTHasBeenInvalidated(token);
            if (isBlacklisted)
            {
               context.Fail("BLACKLISTED TOKEN");
            }
            await base.TokenValidated(context);
        }
    }
}
