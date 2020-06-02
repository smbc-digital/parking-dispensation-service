using StockportGovUK.NetStandard.Models.Addresses;

namespace parking_dispensation_service.Models
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public Address CustomersAddress { get; set; }
    }
}
