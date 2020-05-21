using parking_dispensation_service.Models;
using StockportGovUK.NetStandard.Gateways.VerintServiceGateway;
using StockportGovUK.NetStandard.Models.Verint;
using System;
using System.Threading.Tasks;

namespace parking_dispensation_service.Services
{
    public class ParkingDispensationService : IParkingDispensationService
    {

        private readonly IVerintServiceGateway _VerintServiceGateway;

        public ParkingDispensationService(IVerintServiceGateway verintServiceGateway)
        {
            _VerintServiceGateway = verintServiceGateway;
        }
        public async Task<string> CreateCase(ParkingDispensationRequest parkingDispensationRequest)
        {
            var description = $@"FirstName: {parkingDispensationRequest.FirstName}
                                LastName: { parkingDispensationRequest.LastName}
                                Email: {parkingDispensationRequest.Email}  
                                Phone: {parkingDispensationRequest.Phone}
                                LocationDetails: {parkingDispensationRequest.LocationDetails}
                                PurposeOfDispensation: {parkingDispensationRequest.PurposeOfDispensation}
                                VehicleDetails: {parkingDispensationRequest.VehicleDetails}
                                DispensationDateStart: {parkingDispensationRequest.DispensationDateStart}
                                DispensationDateEnd: {parkingDispensationRequest.DispensationDateEnd}
                                DispensationTimeStart: {parkingDispensationRequest.DispensationTimeStart}
                                DispensationTimeEnd: {parkingDispensationRequest.DispensationTimeEnd}
                                ";

            if (parkingDispensationRequest.CustomersAddress != null)
            {
                description += $@"
                                SelectedAddress: {parkingDispensationRequest.CustomersAddress.SelectedAddress}
                                ";
            }

            var crmCase = new Case
            {
                EventCode = 4000031,
                EventTitle = "Basic Verint Case",
                Description = description,
                Street = new Street
                {
                    Reference = parkingDispensationRequest.StreetAddress?.PlaceRef
                }
            };

            if (!string.IsNullOrEmpty(parkingDispensationRequest.FirstName) && !string.IsNullOrEmpty(parkingDispensationRequest.LastName))
            {
                crmCase.Customer = new Customer
                {
                    Forename = parkingDispensationRequest.FirstName,
                    Surname = parkingDispensationRequest.LastName,
                    Email = parkingDispensationRequest.Email,
                    Mobile = parkingDispensationRequest.Phone
                };

                if (string.IsNullOrEmpty(parkingDispensationRequest.CustomersAddress?.PlaceRef))
                {
                    crmCase.Customer.Address = new Address
                    {
                        AddressLine1 = parkingDispensationRequest.CustomersAddress?.AddressLine1,
                        AddressLine2 = parkingDispensationRequest.CustomersAddress?.AddressLine2,
                        AddressLine3 = parkingDispensationRequest.CustomersAddress?.Town,
                        Postcode = parkingDispensationRequest.CustomersAddress?.Postcode,
                    };
                }
                else
                {
                    crmCase.Customer.Address = new Address
                    {
                        Reference = parkingDispensationRequest.CustomersAddress?.PlaceRef,
                        UPRN = parkingDispensationRequest.CustomersAddress?.PlaceRef
                    };
                }
            }

            try
            {
                var response = await _VerintServiceGateway.CreateCase(crmCase);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception("Status code not successful");
                }

                return response.ResponseContent;
            }
            catch (Exception ex)
            {
                throw new Exception($"CRMService CreateCase an exception has occured while creating the case in verint service", ex);
            }
        }
    }
}

