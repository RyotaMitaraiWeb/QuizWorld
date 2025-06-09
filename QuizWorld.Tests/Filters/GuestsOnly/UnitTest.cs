using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Moq;
using QuizWorld.Infrastructure.Filters.GuestsOnly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using QuizWorld.Web.Contracts.Legacy;

namespace QuizWorld.Tests.Filters.GuestsOnly
{
    public class UnitTest
    {
        public GuestsOnlyFilter filter;
        public Mock<IJwtServiceDeprecated> jwtServiceMock;
        public Mock<IHttpContextAccessor> httpMock;
        public ActionContext actionContext;
        public AuthorizationFilterContext authorizationFilterContext;

        [SetUp]
        public void Setup()
        {
            this.jwtServiceMock = new Mock<IJwtServiceDeprecated>();
            this.httpMock= new Mock<IHttpContextAccessor>();

            this.actionContext = new ActionContext()
            {
                HttpContext = new DefaultHttpContext(),
                RouteData = new RouteData(),
                ActionDescriptor = new ActionDescriptor()
            };

            this.authorizationFilterContext = new AuthorizationFilterContext(
                    this.actionContext,
                    new List<IFilterMetadata>()
                );

            this.filter = new GuestsOnlyFilter(this.jwtServiceMock.Object, this.httpMock.Object);
        }

        [Test]
        public async Task Test_ContextFailsIfTheJWTIsValid()
        {
            this.httpMock
                .Setup(h => h.HttpContext.Request.Headers.Authorization)
                .Returns("bearer jwt");

            this.jwtServiceMock
                .Setup(js => js.RemoveBearer("bearer jwt"))
                .Returns("jwt");

            this.jwtServiceMock
                .Setup(js => js.CheckIfJWTIsValid("jwt"))
                .ReturnsAsync(true);

            await this.filter.OnAuthorizationAsync(this.authorizationFilterContext);
            Assert.That(this.authorizationFilterContext.Result, Is.TypeOf<ForbidResult>());
        }

        [Test]
        public async Task Test_ContextDoesNotFailIfJWTIsInvalid()
        {
            this.httpMock
                .Setup(h => h.HttpContext.Request.Headers.Authorization)
                .Returns("bearer jwt");

            this.jwtServiceMock
                .Setup(js => js.RemoveBearer("bearer jwt"))
                .Returns("jwt");

            this.jwtServiceMock
                .Setup(js => js.CheckIfJWTIsValid("jwt"))
                .ReturnsAsync(false);

            await this.filter.OnAuthorizationAsync(this.authorizationFilterContext);
            Assert.That(this.authorizationFilterContext.Result, Is.Null);
        }

        [Test]
        public async Task Test_ContextResolvesToStatusCodeResultIfJWTServiceThrows()
        {
            this.httpMock
                .Setup(h => h.HttpContext.Request.Headers.Authorization)
                .Returns("bearer jwt");

            this.jwtServiceMock
                .Setup(js => js.RemoveBearer("bearer jwt"))
                .Returns("jwt");

            this.jwtServiceMock
                .Setup(js => js.CheckIfJWTIsValid("jwt"))
                .ThrowsAsync(new Exception());

            await this.filter.OnAuthorizationAsync(this.authorizationFilterContext);
            Assert.That(this.authorizationFilterContext.Result, Is.TypeOf<StatusCodeResult>());
        }
    }
}
