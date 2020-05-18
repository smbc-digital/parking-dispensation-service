using Moq;
using parking_dispensation_service.Models;
using parking_dispensation_service.Services;
using StockportGovUK.NetStandard.Gateways.VerintServiceGateway;
using StockportGovUK.NetStandard.Models.Verint;
using System;
using System.Threading.Tasks;
using Xunit;

namespace parking_dispensation_service_tests.Services
{
    public class ParkingDispensationServiceTests
    {
        private Mock<IVerintServiceGateway> _mockVerintServiceGateway = new Mock<IVerintServiceGateway>();
        private ParkingDispensationService _service;

        public ParkingDispensationServiceTests()
        {
            _service = new ParkingDispensationService(_mockVerintServiceGateway.Object);
        }

        [Fact]
        public async Task CreateCase_ShouldReThrowCreateCaseException_CaughtFromVerintGateway()
        {
            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Throws(new Exception("TestException"));

            _ =  await Assert.ThrowsAsync<Exception>(() => _service.CreateCase(new ParkingDispensationRequest()));
        }

        [Fact]
        public async Task CreateCase_ShouldThrowException_WhenIsNotSuccessStatusCode()
        {
            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .ReturnsAsync(new StockportGovUK.NetStandard.Gateways.Response.HttpResponse<string>
                {
                    IsSuccessStatusCode = false
                });

            _ = await Assert.ThrowsAsync<Exception>(() => _service.CreateCase(new ParkingDispensationRequest()));
        }

        [Fact]
        public async Task CreateCase_ShouldReturnResponseContent()
        {
            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .ReturnsAsync(new StockportGovUK.NetStandard.Gateways.Response.HttpResponse<string>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = "test"
                });

            var result = await _service.CreateCase(new ParkingDispensationRequest());

            Assert.Equal("test", result);
        }

        [Fact]
        public async Task CreateCase_ShouldCallVerintGatewayWithCustomerAddressDescription()
        {
            Case crmCaseParameter = null;

            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Callback<Case>(_ => crmCaseParameter = _)
                .ReturnsAsync(new StockportGovUK.NetStandard.Gateways.Response.HttpResponse<string>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = "test"
                });

            var model = new ParkingDispensationRequest{
                CustomersAddress = new StockportGovUK.NetStandard.Models.Addresses.Address
                {
                    AddressLine1 = "address line 1",
                    AddressLine2 = "address line 2",
                    Town = "town",
                    PlaceRef = "place ref",
                    Postcode = "post code",
                    SelectedAddress = "selected address"
                }
            };

            _ = await _service.CreateCase(model);

            _mockVerintServiceGateway.Verify(_ => _.CreateCase(It.IsAny<Case>()), Times.Once);

            Assert.NotNull(crmCaseParameter);
            Assert.Contains($"AddressLine1: {model.CustomersAddress.AddressLine1}", crmCaseParameter.Description);
            Assert.Contains($"AddressLine2: {model.CustomersAddress.AddressLine2}", crmCaseParameter.Description);
            Assert.Contains($"Town: {model.CustomersAddress.Town}", crmCaseParameter.Description);
            Assert.Contains($"Postcode: {model.CustomersAddress.Postcode}", crmCaseParameter.Description);
            Assert.Contains($"SelectedAddress: {model.CustomersAddress.SelectedAddress}", crmCaseParameter.Description);
        }

        [Fact]
        public async Task CreateCase_ShouldCallVerintGatewayWithCustomer()
        {
            Case crmCaseParameter = null;

            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Callback<Case>(_ => crmCaseParameter = _)
                .ReturnsAsync(new StockportGovUK.NetStandard.Gateways.Response.HttpResponse<string>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = "test"
                });

            var model = new ParkingDispensationRequest
            {
                FirstName = "first name",
                LastName = "last name",
                Email = "test@test.com",
                Phone = "+447777777777"
            };

            _ = await _service.CreateCase(model);

            _mockVerintServiceGateway.Verify(_ => _.CreateCase(It.IsAny<Case>()), Times.Once);

            Assert.NotNull(crmCaseParameter);
            Assert.Equal(model.Email, crmCaseParameter.Customer.Email);
            Assert.Equal(model.Phone, crmCaseParameter.Customer.Mobile);
            Assert.Equal(model.LastName, crmCaseParameter.Customer.Surname);
            Assert.Equal(model.FirstName, crmCaseParameter.Customer.Forename);
        }

        [Fact]
        public async Task CreateCase_ShouldCallVerintGatewayWithCustomerManualAddress()
        {
            Case crmCaseParameter = null;

            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Callback<Case>(_ => crmCaseParameter = _)
                .ReturnsAsync(new StockportGovUK.NetStandard.Gateways.Response.HttpResponse<string>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = "test"
                });

            var model = new ParkingDispensationRequest
            {
                FirstName = "first name",
                LastName = "last name",
                CustomersAddress = new StockportGovUK.NetStandard.Models.Addresses.Address
                {
                    AddressLine1 = "address line 1",
                    AddressLine2 = "address line 2",
                    Town = "town",
                    Postcode = "post code",
                }
            };

            _ = await _service.CreateCase(model);

            _mockVerintServiceGateway.Verify(_ => _.CreateCase(It.IsAny<Case>()), Times.Once);

            Assert.NotNull(crmCaseParameter);
            Assert.Equal(model.CustomersAddress.AddressLine1, crmCaseParameter.Customer.Address.AddressLine1);
            Assert.Equal(model.CustomersAddress.AddressLine2, crmCaseParameter.Customer.Address.AddressLine2);
            Assert.Equal(model.CustomersAddress.Town, crmCaseParameter.Customer.Address.AddressLine3);
            Assert.Equal(model.CustomersAddress.Postcode, crmCaseParameter.Customer.Address.Postcode);
            Assert.Null(crmCaseParameter.Customer.Address.UPRN);
            Assert.Null(crmCaseParameter.Customer.Address.Reference);
        }

        [Fact]
        public async Task CreateCase_ShouldCallVerintGatewayWithCustomerAutomaticAddress()
        {
            Case crmCaseParameter = null;

            _mockVerintServiceGateway
                .Setup(_ => _.CreateCase(It.IsAny<Case>()))
                .Callback<Case>(_ => crmCaseParameter = _)
                .ReturnsAsync(new StockportGovUK.NetStandard.Gateways.Response.HttpResponse<string>
                {
                    IsSuccessStatusCode = true,
                    ResponseContent = "test"
                });

            var model = new ParkingDispensationRequest
            {
                FirstName = "first name",
                LastName = "last name",
                CustomersAddress = new StockportGovUK.NetStandard.Models.Addresses.Address
                {
                    PlaceRef = "test place ref"
                }
            };

            _ = await _service.CreateCase(model);

            _mockVerintServiceGateway.Verify(_ => _.CreateCase(It.IsAny<Case>()), Times.Once);

            Assert.NotNull(crmCaseParameter);
            Assert.Null(crmCaseParameter.Customer.Address.AddressLine1);
            Assert.Null(crmCaseParameter.Customer.Address.AddressLine2);
            Assert.Null(crmCaseParameter.Customer.Address.AddressLine3);
            Assert.Null(crmCaseParameter.Customer.Address.Postcode);
            Assert.Equal(model.CustomersAddress.PlaceRef, crmCaseParameter.Customer.Address.UPRN);
            Assert.Equal(model.CustomersAddress.PlaceRef, crmCaseParameter.Customer.Address.Reference);
        }
    }
}
