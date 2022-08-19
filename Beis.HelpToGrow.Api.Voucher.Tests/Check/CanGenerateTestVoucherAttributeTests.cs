using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace Beis.HelpToGrow.Api.Voucher.Tests.Check
{
    [TestFixture]
    public class CanGenerateTestVoucherAttributeTests
    {
        private Mock<Microsoft.AspNetCore.Routing.RouteData> _mockRouteData;
        ActionExecutingContext _context;

        [SetUp]
        public void Setup()
        {
            _mockRouteData = new Mock<Microsoft.AspNetCore.Routing.RouteData>();
            var modelState = new ModelStateDictionary();
            var httpContext = new DefaultHttpContext();
            _context = new ActionExecutingContext(
                new ActionContext(
                    httpContext: httpContext,
                    routeData: _mockRouteData.Object,
                    actionDescriptor: new ActionDescriptor(),
                    modelState: modelState
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object);
        }

        [Test]
        public void CanGenerateTestVoucherAttribute_returns_404_for_false_env_var()
        {
            //Arrange

            var sut = new CanGenerateTestVoucherAttribute(false);

            //Act
            sut.OnActionExecuting(_context);

            //Assert
            Assert.NotNull(_context.Result);
            Assert.AreEqual(_context.Result.GetType(), typeof(NotFoundResult));
        }

        [Test]
        public void CanGenerateTestVoucherAttribute_returns_ok_for_false_env_var()
        {
            //Arrange
            var sut = new CanGenerateTestVoucherAttribute(true);

            //Act
            sut.OnActionExecuting(_context);

            //Assert
            Assert.IsNull(_context.Result);
        }
    }
}