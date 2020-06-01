using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using parking_dispensation_service.Helpers;
using parking_dispensation_service.Models;
using StockportGovUK.NetStandard.Gateways.VerintServiceGateway;
using StockportGovUK.NetStandard.Models.Enums;
using StockportGovUK.NetStandard.Models.Verint;

namespace parking_dispensation_service.Services
{
    public class ParkingDispensationService : IParkingDispensationService
    {

        private readonly IVerintServiceGateway _VerintServiceGateway;
        private readonly IMailHelper _mailHelper;

        public ParkingDispensationService(IVerintServiceGateway verintServiceGateway
                                        , IMailHelper mailHelper)
        {
            _VerintServiceGateway = verintServiceGateway;
            _mailHelper = mailHelper;
        }


        public ParkingDispensationService(IVerintServiceGateway verintServiceGateway)
        {
            _VerintServiceGateway = verintServiceGateway;
        }
        public async Task<string> CreateCase(ParkingDispensationRequest parkingDispensationRequest)
        {
           var description = $@"Reason: {parkingDispensationRequest.PurposeOfDispensation}
                                Start date: {parkingDispensationRequest.DispensationDateStart.ToString("dd/MM/yyyy")}
                                End date: {parkingDispensationRequest.DispensationDateEnd.ToString("dd/MM/yyyy")}
                                Start time: {parkingDispensationRequest.DispensationTimeStart.ToString("HH:mm")}
                                End time: {parkingDispensationRequest.DispensationTimeEnd.ToString("HH:mm")}
                                Vehicle information: {parkingDispensationRequest.VehicleDetails}
                                Further location information: {parkingDispensationRequest.LocationDetails}
                                ";

            var crmCase = new Case
            {
                EventCode = 2002798,
                EventTitle = "Request for NON enforcement",
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
                };

                if (!string.IsNullOrEmpty(parkingDispensationRequest.Email))
                {
                    crmCase.Customer.Email = parkingDispensationRequest.Email;
                }

                if (!string.IsNullOrEmpty(parkingDispensationRequest.Phone))
                {
                    crmCase.Customer.Mobile = parkingDispensationRequest.Phone;
                }

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

                Person person = new Person
                {
                    FirstName = parkingDispensationRequest.FirstName,   
                    Email = parkingDispensationRequest.Email,
                };

                _mailHelper.SendEmail(person, EMailTemplate.ParkingDispensationRequest, response.ResponseContent);
                return response.ResponseContent;
            }
            catch (Exception ex)
            {
                throw new Exception($"CRMService CreateCase an exception has occured while creating the case in verint service", ex);
            }
        }
    }
}

