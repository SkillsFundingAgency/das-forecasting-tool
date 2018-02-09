using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Domain.Commitments;

namespace SFA.DAS.Forecasting.Application.Commitments.Handlers
{
    public class RecordCommitmentHandler
    {
        private readonly EmployerCommitmentsRepository _commitmentsRepository;

        public RecordCommitmentHandler(EmployerCommitmentsRepository commitmentsRepository)
        {
            _commitmentsRepository = commitmentsRepository ?? throw new ArgumentNullException(nameof(commitmentsRepository));
        }

        public async Task Handle(PaymentEvent paymentEvent)
        {
            var employerCommitments = await _commitmentsRepository.Get(paymentEvent.EmployerAccountId);
            employerCommitments.AddCommitment(paymentEvent.ApprenticeshipId, paymentEvent.Uln, paymentEvent.EarningDetails.StartDate, 
                paymentEvent.EarningDetails.PlannedEndDate, paymentEvent.EarningDetails.ActualEndDate, 
                paymentEvent.EarningDetails.MonthlyInstallment, paymentEvent.EarningDetails.CompletionAmount, 
                (short)paymentEvent.EarningDetails.TotalInstallments);
            await _commitmentsRepository.Store(employerCommitments);
        }
    }
}