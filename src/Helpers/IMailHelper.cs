using parking_dispensation_service.Models;
using StockportGovUK.NetStandard.Models.Enums;

namespace parking_dispensation_service.Helpers
{
    public interface IMailHelper
    {
        void SendEmail(Person person, EMailTemplate template, string caseReference);
    }
}