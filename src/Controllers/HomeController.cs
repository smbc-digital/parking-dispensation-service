using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using parking_dispensation_service.Models;
using parking_dispensation_service.Services;
using StockportGovUK.AspNetCore.Attributes.TokenAuthentication;
using System.Threading.Tasks;

namespace parking_dispensation_service.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[Controller]")]
    [ApiController]
    [TokenAuthentication]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IParkingDispensationService _parkingDispensationRequest;

        public HomeController(ILogger<HomeController> logger, IParkingDispensationService parkingDispensationRequest)
        {
            _logger = logger;
            _parkingDispensationRequest = parkingDispensationRequest;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ParkingDispensationRequest parkingDispensationRequest)
            => Ok(await _parkingDispensationRequest.CreateCase(parkingDispensationRequest));
    }
}