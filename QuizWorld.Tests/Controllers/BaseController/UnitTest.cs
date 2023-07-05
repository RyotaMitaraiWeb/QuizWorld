using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using QuizWorld.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Tests.Controllers.BaseControllerTests
{
    public class UnitTest
    {
        public BaseController controller { get; set; }
        public DefaultHttpContext httpContext { get; set; }

        [SetUp]
        public void Setup()
        {
            this.httpContext = new DefaultHttpContext();
            this.controller = new BaseController()
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = this.httpContext
                }
            };
        }

        [Test]
        public void Test_GetJWTReturnsTheStringInAuthorizationHeader()
        {
            this.httpContext.Request.Headers["Authorization"] = "a";
            string jwt = this.controller.GetJWT();
            Assert.That(jwt, Is.EqualTo("a"));
        }

        [Test]
        public void Test_GetJWTReturnsAnEmptyStringIfAuthorizationHeaderIsMissing()
        {
            // ensure the header is deleted
            this.httpContext.Request.Headers.Remove("Authorization");

            string jwt = this.controller.GetJWT();
            Assert.That(jwt, Is.EqualTo(string.Empty));
        }
    }
}
