using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Beis.HelpToGrow.Api.Voucher.Tests.Check
{
    [TestFixture]
    public class CanGenerateTestVoucherAttributeTests
    {
        public const string canGenerateTestVoucherEnvVar = "SHOW_GENERATE_TEST_VOUCHER_ENDPOINT";
        private Mock<Microsoft.AspNetCore.Routing.RouteData> mockRouteData;
        ActionExecutingContext context;

        [SetUp]
        public void Setup()
        {
             mockRouteData = new Mock<Microsoft.AspNetCore.Routing.RouteData>();
            var modelState = new ModelStateDictionary();
            var httpContext = new DefaultHttpContext();
            context = new ActionExecutingContext(
                new ActionContext(
                    httpContext: httpContext,
                    routeData: mockRouteData.Object,
                    actionDescriptor: new ActionDescriptor(),
                    modelState: modelState
                ),
                new List<IFilterMetadata>(),
                new Dictionary<string, object>(),
                new Mock<Controller>().Object);

        }

        [Test]
        public void CanGenerateTestVoucherAttribute_returns_404_for_no_env_var()
        {             
            //Arrange
           
            var sut = new CanGenerateTestVoucherAttribute();

            //Act
            sut.OnActionExecuting(context);

            //Assert
            Assert.AreEqual(context.Result.GetType(), typeof(NotFoundResult));
           
        }

        [Test]
        public void CanGenerateTestVoucherAttribute_returns_404_for_false_env_var()
        {

            Environment.SetEnvironmentVariable(canGenerateTestVoucherEnvVar, "false");

            //Arrange

            var sut = new CanGenerateTestVoucherAttribute();

            //Act
            sut.OnActionExecuting(context);

            //Assert
            Assert.AreEqual(context.Result.GetType(), typeof(NotFoundResult));

        }


        [Test]
        public void CanGenerateTestVoucherAttribute_returns_ok_for_false_env_var()
        {

            Environment.SetEnvironmentVariable(canGenerateTestVoucherEnvVar, "true");

            //Arrange

            var sut = new CanGenerateTestVoucherAttribute();

            //Act
            sut.OnActionExecuting(context);

            //Assert
            Assert.IsNull(context.Result);

        }
    }
}
