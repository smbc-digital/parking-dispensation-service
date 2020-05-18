using Microsoft.Extensions.Logging;
using Moq;
using parking_dispensation_service.Controllers;
using parking_dispensation_service.Models;
using parking_dispensation_service.Services;
using System.Threading.Tasks;
using Xunit;

namespace parking_dispensation_service_tests.Controller
{
    public class HomeControllerTests
    {
        private HomeController _homeController;
        private Mock<IParkingDispensationService> _mockParkingDispensationService = new Mock<IParkingDispensationService>();

        public HomeControllerTests()
        {
            _homeController = new HomeController(Mock.Of<ILogger<HomeController>>(), _mockParkingDispensationService.Object);
        }

        [Fact]
        public async Task Post_ShouldCallCreateCase()
        {
            _mockParkingDispensationService
                .Setup(_ => _.CreateCase(It.IsAny<ParkingDispensationRequest>()))
                .ReturnsAsync("test");

            var result = await _homeController.Post(null);

            _mockParkingDispensationService
                .Verify(_ => _.CreateCase(null), Times.Once);
        }

        [Fact]
        public async Task Post_ReturnOkActionResult()
        {
            _mockParkingDispensationService
                .Setup(_ => _.CreateCase(It.IsAny<ParkingDispensationRequest>()))
                .ReturnsAsync("test");

            var result = await _homeController.Post(null);

            Assert.Equal("OkObjectResult", result.GetType().Name);
        }
    }
}