using Microsoft.AspNetCore.Authentication.JwtBearer;
using QuizWorld.Common.ApiVersion;
using QuizWorld.Common.Hubs;
using QuizWorld.Common.Util;
using QuizWorld.Web.Contracts;
using QuizWorld.Web.Contracts.Legacy;


namespace QuizWorld.Infrastructure.AuthConfig
{
    /// <summary>
    /// Configures the JWT middleware to refer to the JWT blacklist when validating the token
    /// </summary>
    public class AppJwtBearerEvents(IJwtServiceDeprecated jwtService, IJwtStore jwtStore) : JwtBearerEvents
    {
        private readonly IJwtServiceDeprecated jwtService = jwtService;
        private readonly IJwtStore _jwtStore = jwtStore;

        public override async Task TokenValidated(TokenValidatedContext context)
        {
            string? bearer = context.Request.Headers.Authorization.FirstOrDefault() ?? context.Request.Query["access_token"];
            if (string.IsNullOrWhiteSpace(bearer))
            {
                context.Fail("Missing header Authorization");
                return;
            }

            string apiVersion = context
                .Request
                .Query[ApiVersion.QueryStringName]
                .ToString()
                    ?? ApiVersion.DefaultVersion;

            if (apiVersion == "1.0")
            {
                await TokenValidatedV1(context);
                return;
            }

            await ValidateJwt(context, bearer);
            return;
        }

        public override async Task MessageReceived(MessageReceivedContext context)
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments(HubEndpoints.HubsPrefix)))
            {
                context.Token = accessToken;
            }

            await base.MessageReceived(context);
        }

        private async Task ValidateJwt(TokenValidatedContext context, string bearer)
        {
            string jwt = JwtUtil.RemoveBearer(bearer);
            var result = await _jwtStore.RetrieveBlacklistedTokenAsync(jwt);

            if (result.IsFailure)
            {
                await base.TokenValidated(context);
                return;
            }

            context.Fail(BlacklistedTokenErrorMessage);
            return;
        }

        /// <summary>
        /// This should only be used for the deprecated v1 of the API until support
        /// is completely abandoned for it. For the new version, use ValidateJwt instead
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>

        private async Task TokenValidatedV1(TokenValidatedContext context)
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

        private readonly string BlacklistedTokenErrorMessage = "The token has been blacklisted and cannot be authorized";
    }
}
