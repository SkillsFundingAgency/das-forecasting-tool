using Nancy;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub
{
    public class ProviderEventApiModule : NancyModule
    {
        public ProviderEventApiModule()
        {
            Get("/api/payments", x => {
                if (!string.IsNullOrEmpty(x.accountId))
                {
                    return JsonConvert.SerializeObject(ProviderEventTestData.GetPayment(x.accountId, x.period_id, x.page_nubmber));
                }
                return JsonConvert.SerializeObject(ProviderEventTestData.GetPayment(x.accountId, x.period_id, x.page_nubmber));
            });

            //    api/payments?page={page}&periodId={periodId}&employerAccountId={employerAccountId}&ukprn={ukprn}");
            //         public async Task<PageOfResults<Payment>> GetPayments(string periodId = null, string employerAccountId = null, int page = 1, long? ukprn = null)

            Get("/api/payments?page={page_number}&periodId={period_id}&employerAccountId={accountId}&ukprn=", x => {
                if (!string.IsNullOrEmpty(x.accountId))
                {
                    return JsonConvert.SerializeObject(ProviderEventTestData.GetPayment(x.accountId, x.period_id, x.page_nubmber));
                }

                return "";
            });
        }
    }
}
