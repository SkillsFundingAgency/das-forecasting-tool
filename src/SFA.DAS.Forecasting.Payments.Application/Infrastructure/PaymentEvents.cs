using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Payments.Domain.Entities;
using SFA.DAS.Forecasting.Payments.Domain.Interfaces;
using SFA.DAS.Forecasting.Payments.Messages.Events;
using SFA.DAS.Provider.Events.Api.Client;

namespace SFA.DAS.Forecasting.Payments.Application.Infrastructure
{
    public class PaymentEvents : IPaymentEvents
    {
        private PaymentsEventsApiClient _client;
        private readonly TableStorage _table;
        private readonly PaymentEventsApiConfig _config;

        public PaymentEvents(PaymentEventsApiConfig config)
        {
            _client = new PaymentsEventsApiClient(config);
            _table = new TableStorage(config.StorageConnectionString);
            _config = config;
        }

        public async Task<IEnumerable<PaymentMessage>> ReadAsync()
        {
            var info = await _table.GetLatestInfo();
            if (info.PageNumber <= _config.MaxPages)
            {
                var result = await _client.GetPayments(page: info.PageNumber + 1);

                await _table.Insert(new PaymentsAdapter.Functions.Infrastructure.Models.Info { PageNumber = result.PageNumber });
                return result.Items.Select(MapToPaymentMessage);
            }
            
            return new List<PaymentMessage>();
        }

        private PaymentMessage MapToPaymentMessage(Provider.Events.Api.Types.Payment payment)
        {
            return new PaymentMessage
            {
                Id = payment.Id,
                EmployerAccountId = payment.EmployerAccountId,
                Ukprn = payment.Ukprn,
                ApprenticeshipId = payment.ApprenticeshipId,
                Uln = payment.Uln,
                StandardCode = payment.StandardCode,

                ProgrammeType = payment.ProgrammeType,
                FrameworkCode = payment.FrameworkCode,
                PathwayCode = payment.PathwayCode,

                Amount = payment.Amount
            };
        }

        //public async Task<IEnumerable<PaymentMessage>> ReadAsync()
        //{
        //    var l = new List<PaymentMessage> {
        //        new PaymentMessage { Id = Guid.NewGuid().ToString("N"), EmployerAccountId = "12345"},
        //        new PaymentMessage { Id = Guid.NewGuid().ToString("N"), EmployerAccountId = "ABCDE" }
        //    };

        //    return await Task.FromResult(l);
        //}
    }
}
