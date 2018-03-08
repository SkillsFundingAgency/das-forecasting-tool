using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Commitments.Handlers
{
    public class StoreCommitmentHandler
    {
        private readonly IEmployerCommitmentsRepository _repository;
        private readonly ILog _logger;

        public StoreCommitmentHandler(IEmployerCommitmentsRepository repository, ILog logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PaymentCreatedMessage message)
        {
            if (message.EarningDetails == null)
                throw new InvalidOperationException($"Invalid payment created message. Earning details is null so cannot create commitment data. Employer account: {message.EmployerAccountId}, payment id: {message.Id}");

            var employerCommitments = await _repository.Get(message.EmployerAccountId);
            employerCommitments.AddCommitment(message.ApprenticeshipId, message.Uln, message.ApprenticeName, 
                message.CourseName, message.CourseLevel, message.Ukprn, message.ProviderName, 
                message.EarningDetails.StartDate, message.EarningDetails.PlannedEndDate, 
                message.EarningDetails.ActualEndDate, message.EarningDetails.MonthlyInstallment, 
                message.EarningDetails.CompletionAmount, (short)message.EarningDetails.TotalInstallments);
            _logger.Debug($"Now storing the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.Id}");
            await _repository.Store(employerCommitments);
            _logger.Info($"Finished adding the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.Id}");
        }
    }
}