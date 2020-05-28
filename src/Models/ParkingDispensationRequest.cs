using System;

namespace parking_dispensation_service.Models
{
    public class ParkingDispensationRequest
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LocationDetails { get; set; }
        public string PurposeOfDispensation { get; set; }
        public string VehicleDetails { get; set; }
        public DateTime DispensationDateStart { get; set; }
        public DateTime DispensationDateEnd { get; set; }
        public DateTime DispensationTimeStart { get; set; }
        public DateTime DispensationTimeEnd { get; set; }
        public StockportGovUK.NetStandard.Models.Addresses.Address StreetAddress { get; set; }
        public StockportGovUK.NetStandard.Models.Addresses.Address CustomersAddress { get; set; }
        public Person Reporter { get; set; }
    }
}
