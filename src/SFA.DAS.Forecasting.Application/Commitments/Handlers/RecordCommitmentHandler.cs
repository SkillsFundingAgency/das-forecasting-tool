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

        public async Task Handle(PaymentCreatedMessage paymentCreatedMessage)
        {
            var employerCommitments = await _commitmentsRepository.Get(paymentCreatedMessage.EmployerAccountId);
            employerCommitments.AddCommitment(paymentCreatedMessage.ApprenticeshipId, paymentCreatedMessage.Uln, paymentCreatedMessage.EarningDetails.StartDate, 
                paymentCreatedMessage.EarningDetails.PlannedEndDate, paymentCreatedMessage.EarningDetails.ActualEndDate, 
                paymentCreatedMessage.EarningDetails.MonthlyInstallment, paymentCreatedMessage.EarningDetails.CompletionAmount, 
                (short)paymentCreatedMessage.EarningDetails.TotalInstallments);
            await _commitmentsRepository.Store(employerCommitments);
        }
    }
}