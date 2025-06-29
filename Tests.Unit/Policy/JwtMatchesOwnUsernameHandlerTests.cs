using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using NSubstitute;
using QuizWorld.Infrastructure.AuthConfig.Handlers;
using QuizWorld.Infrastructure.AuthConfig.Requirements;
using QuizWorld.Common.Claims;
using Microsoft.AspNetCore.Routing;

namespace QuizWorld.Tests.AuthConfig.Handlers
{
    [TestFixture]
    public class JwtMatchesOwnUsernameHandlerTests
    {
        [Test]
        public async Task Should_Succeed_When_Username_Matches()
        {
            var username = "testuser";
            var requirement = new JwtMatchesOwnUsernameRequirement();

            var claims = new[] { new Claim(UserClaimsProperties.Username, username) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.RouteValues.Returns(new RouteValueDictionary { { "username", username } });

            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns(httpContext);

            var handler = new JwtMatchesOwnUsernameHandler(httpContextAccessor);

            var authContext = new AuthorizationHandlerContext(new[] { requirement }, user, null);

            await handler.HandleAsync(authContext);

            Assert.That(authContext.HasSucceeded, Is.True);
        }

        [Test]
        public async Task Should_Fail_When_Username_Does_Not_Match()
        {
            var tokenUsername = "testuser";
            var routeUsername = "otheruser";
            var requirement = new JwtMatchesOwnUsernameRequirement();

            var claims = new[] { new Claim(UserClaimsProperties.Username, tokenUsername) };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            var user = new ClaimsPrincipal(identity);

            var httpContext = Substitute.For<HttpContext>();
            httpContext.Request.RouteValues.Returns(new RouteValueDictionary { { "username", routeUsername } });

            var httpContextAccessor = Substitute.For<IHttpContextAccessor>();
            httpContextAccessor.HttpContext.Returns(httpContext);

            var handler = new JwtMatchesOwnUsernameHandler(httpContextAccessor);

            var authContext = new AuthorizationHandlerContext(new[] { requirement }, user, null);

            await handler.HandleAsync(authContext);

            Assert.That(authContext.HasFailed, Is.True);
        }
    }
}