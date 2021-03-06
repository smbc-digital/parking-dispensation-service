﻿using parking_dispensation_service.Models;
using Newtonsoft.Json;
using StockportGovUK.NetStandard.Gateways.MailingServiceGateway;
using StockportGovUK.NetStandard.Models.Mail;
using StockportGovUK.NetStandard.Models.Enums;

namespace parking_dispensation_service.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IMailingServiceGateway _mailingServiceGateway;

        public MailHelper(IMailingServiceGateway mailingServiceGateway)
        {
            _mailingServiceGateway = mailingServiceGateway;
        }

        public void SendEmail(Person person, EMailTemplate template, string caseReference)
        {
            _mailingServiceGateway.Send(new Mail
            {
                Template = template,
                Payload = JsonConvert.SerializeObject(new {
                    Subject = "Parking dispensation request form - submission",
                    Reference = caseReference,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    RecipientAddress = person.Email
                })
            });
        }
    }
}
