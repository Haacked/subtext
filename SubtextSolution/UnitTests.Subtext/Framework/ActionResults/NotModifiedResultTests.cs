﻿using System.Web;
using System.Web.Mvc;
using MbUnit.Framework;
using Moq;
using Moq.Stub;
using Subtext.Framework.Infrastructure.ActionResults;

namespace UnitTests.Subtext.Framework.ActionResults
{
    [TestFixture]
    public class NotModifiedResultTests
    {
        [Test]
        public void NotModifiedResultSends304StatusCodeAndSuppressesContent() {
            // arrange
            var result = new NotModifiedResult();
            var httpContext = new Mock<HttpContextBase>();
            int statusCode = 0;
            
            httpContext.SetupSet(h => h.Response.StatusCode, It.IsAny<int>()).Callback(status => statusCode = status);
            httpContext.Stub(h => h.Response.StatusCode);
            httpContext.Stub(h => h.Response.SuppressContent);
            var controllerContext = new ControllerContext();
            controllerContext.HttpContext = httpContext.Object;

            // act
            result.ExecuteResult(controllerContext);

            // assert
            Assert.AreEqual(304, httpContext.Object.Response.StatusCode);
            Assert.IsTrue(httpContext.Object.Response.SuppressContent);
        }
    }
}
