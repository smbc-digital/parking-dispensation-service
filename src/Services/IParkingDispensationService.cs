using parking_dispensation_service.Models;
using System.Threading.Tasks;

namespace parking_dispensation_service.Services
{
    public interface IParkingDispensationService
    {
        Task<string> CreateCase(ParkingDispensationRequest parkingDispensationRequest);
    }
}
