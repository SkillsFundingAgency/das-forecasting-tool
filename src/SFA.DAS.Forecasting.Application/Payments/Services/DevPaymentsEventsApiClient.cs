using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Provider.Events.Api.Client;
using SFA.DAS.Provider.Events.Api.Client.Configuration;
using SFA.DAS.Provider.Events.Api.Types;

namespace SFA.DAS.Forecasting.Application.Payments.Services
{
    public class DevPaymentsEventsApiClient: IPaymentsEventsApiClient
    {
        private readonly IPaymentsEventsApiConfiguration _configuration;
        private static readonly HttpClient HttpClient = new HttpClient();
        private readonly Uri _uri;
        public DevPaymentsEventsApiClient(IPaymentsEventsApiConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _uri = new Uri(configuration.ApiBaseUrl);
        }

        public Task<PeriodEnd[]> GetPeriodEnds()
        {
            throw new NotImplementedException();
        }

        public async Task<PageOfResults<Payment>> GetPayments(string periodId = null, string employerAccountId = null, int page = 1, long? ukprn = null)
        {
            var uri = $"{_configuration.ApiBaseUrl}api/payments?page={page}&periodId={periodId}&employerAccountId={employerAccountId}&ukprn={ukprn}&code={_configuration.ClientToken}";
            return JsonConvert.DeserializeObject<PageOfResults<Payment>>(await HttpClient.GetStringAsync(uri));
        }

        public Task<PageOfResults<AccountTransfer>> GetTransfers(string periodId = null, long? senderAccountId = null, long? receiverAccountId = null, int page = 1)
        {
            throw new NotImplementedException();
        }

        public Task<PageOfResults<SubmissionEvent>> GetSubmissionEvents(long sinceEventId = 0, DateTime? sinceTime = null, long ukprn = 0, int page = 1)
        {
            throw new NotImplementedException();
        }

        public Task<PageOfResults<DataLockEvent>> GetDataLockEvents(long sinceEventId = 0, DateTime? sinceTime = null, string employerAccountId = null,
            long ukprn = 0, int page = 1)
        {
            throw new NotImplementedException();
        }

        public Task<PaymentStatistics> GetPaymentStatistics()
        {
            throw new NotImplementedException();
        }
    }
}