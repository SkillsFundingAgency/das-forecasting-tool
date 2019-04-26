using System.Threading.Tasks;
using NLog;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public class AggregatePaymentDataHandler
    {
        private readonly IEmployerPaymentDataSession _employerPaymentDataSession;
        private readonly ILog _logger;

        public AggregatePaymentDataHandler(IEmployerPaymentDataSession employerPaymentDataSession, ILog logger)
        {
            _employerPaymentDataSession = employerPaymentDataSession;
            _logger = logger;
        }

        public async Task Handle(AggregatePaymentDataCommand command)
        {
            await _employerPaymentDataSession.CalculatePaymentTotals(command.EmployerAccountId,
                command.CollectionPeriodYear, command.CollectionPeriodMonth);
        }
    }
}