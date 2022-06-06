using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Beis.HelpToGrow.Api.Voucher.Tests.Cancellation
{
    [TestFixture]
    internal class VoucherCancellationControllerTests
    {
        private CancellationResponse cancellationResponse;
        private Mock<IVoucherCancellationService> voucherCancellationService;
        private Mock<ILogger<VoucherCancellationController>> logger;       
        private Mock<IVendorAPICallStatusServices> vendorAPICallStatusServices;
        private VoucherCancellationController controller;




        [SetUp]
        public async Task Setup()
        {
            voucherCancellationService = new Mock<IVoucherCancellationService>();
            voucherCancellationService.Setup(x => x.CancelVoucherFromVoucherCode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync((string voucherCode, string vendorRegistration, string vendorAccessCode) =>  cancellationResponse);

            logger = new Mock<ILogger<VoucherCancellationController>>();
            vendorAPICallStatusServices = new Mock<IVendorAPICallStatusServices>();
            controller = new VoucherCancellationController(logger.Object, voucherCancellationService.Object, vendorAPICallStatusServices.Object);
        }


        [Test]
        public async Task ReturnsSuccessfullyCancelledResponse()
        {
            //Arrange
            cancellationResponse = CancellationResponse.SuccessfullyCancelled;

            //Act
            var result = await controller.Post(new VoucherCancellationRequest { VoucherCode = "abc123"});

            //Assert
            Assert.AreEqual(0, result.Value.ErrorCode);
            Assert.AreEqual("Successfully cancelled", result.Value.Message);
            Assert.AreEqual("abc123", result.Value.VoucherCode);
        }


        [Test]
        public async Task ReturnsAlreadyCancelledResponse()
        {
            //Arrange
            cancellationResponse = CancellationResponse.AlreadyCancelled;

            //Act
            var result = await controller.Post(new VoucherCancellationRequest { VoucherCode = "abc123" });

            //Assert
            Assert.AreEqual(0, result.Value.ErrorCode);
            Assert.AreEqual("Voucher already cancelled", result.Value.Message);
            Assert.AreEqual("abc123", result.Value.VoucherCode);
        }


        [Test]
        public async Task ReturnsFreeTrialExpiredResponse()
        {
            //Arrange
            cancellationResponse = CancellationResponse.FreeTrialExpired;

            //Act
            var result = await controller.Post(new VoucherCancellationRequest { VoucherCode = "abc123" });

            //Assert
            Assert.AreEqual(0, result.Value.ErrorCode);
            Assert.AreEqual("Voucher already cancelled. SME cannot reapply", result.Value.Message);
            Assert.AreEqual("abc123", result.Value.VoucherCode);
        }

        [Test]
        public async Task ReturnsTokenExpiredResponse()
        {
            //Arrange
            cancellationResponse = CancellationResponse.TokenExpired;

            //Act
            var result = await controller.Post(new VoucherCancellationRequest { VoucherCode = "abc123" });

            //Assert
            Assert.AreEqual(0, result.Value.ErrorCode);
            Assert.AreEqual("Voucher already expired", result.Value.Message);
            Assert.AreEqual("abc123", result.Value.VoucherCode);
        }


        [Test]
        public async Task ReturnsUnknownVoucherCodeResponse()
        {
            //Arrange
            cancellationResponse = CancellationResponse.UnknownVoucherCode;

            //Act
            var result = await controller.Post(new VoucherCancellationRequest { VoucherCode = "abc123" });
            VoucherCancellationResponse voucherResponse = (VoucherCancellationResponse)(result.Result as ObjectResult).Value;
            //Assert
            Assert.AreEqual(10, voucherResponse.ErrorCode);
            Assert.AreEqual("Unknown Voucher", voucherResponse.Message);
            Assert.AreEqual("abc123", voucherResponse.VoucherCode);
        }

        [Test]
        public async Task ReturnsTokenNotFoundResponse()
        {
            //Arrange
            cancellationResponse = CancellationResponse.TokenNotFound;

            //Act
            var result = await controller.Post(new VoucherCancellationRequest { VoucherCode = "abc123" });
            VoucherCancellationResponse voucherResponse = (VoucherCancellationResponse)(result.Result as ObjectResult).Value;

            //Assert
            Assert.AreEqual(10, voucherResponse.ErrorCode);
            Assert.AreEqual("Unknown Voucher", voucherResponse.Message);
            Assert.AreEqual("abc123", voucherResponse.VoucherCode);
        }

        [Test]
        public async Task ReturnsUnknownVendorRegistrationResponse()
        {
            //Arrange
            cancellationResponse = CancellationResponse.UnknownVendorRegistration;

            //Act
            var result = await controller.Post(new VoucherCancellationRequest { VoucherCode = "abc123" });
            VoucherCancellationResponse voucherResponse = (VoucherCancellationResponse)(result.Result as ObjectResult).Value;

            //Assert
            Assert.AreEqual(20, voucherResponse.ErrorCode);
            Assert.AreEqual("Unknown Vendor", voucherResponse.Message);
            Assert.AreEqual("abc123", voucherResponse.VoucherCode);
        }

        [Test]
        public async Task ReturnsUnknownVendorAccessCodeResponse()
        {
            //Arrange
            cancellationResponse = CancellationResponse.UnknownVendorAccessCode;

            //Act
            var result = await controller.Post(new VoucherCancellationRequest { VoucherCode = "abc123" });
            VoucherCancellationResponse voucherResponse = (VoucherCancellationResponse)(result.Result as ObjectResult).Value;

            //Assert
            Assert.AreEqual(30, voucherResponse.ErrorCode);
            Assert.AreEqual("Unknown Access Code", voucherResponse.Message);
            Assert.AreEqual("abc123", voucherResponse.VoucherCode);
        }

        [Test]
        public async Task ReturnsUnknownErrorResponse()
        {
            //Arrange
            cancellationResponse = CancellationResponse.UnknownError;

            //Act
            var result = await controller.Post(new VoucherCancellationRequest { VoucherCode = "abc123" });
            VoucherCancellationResponse voucherResponse = (VoucherCancellationResponse)(result.Result as ObjectResult).Value;

            //Assert
            Assert.AreEqual(40, voucherResponse.ErrorCode);
            Assert.AreEqual("Unknown Error", voucherResponse.Message);
            Assert.AreEqual("abc123", voucherResponse.VoucherCode);
        }
    }
}
