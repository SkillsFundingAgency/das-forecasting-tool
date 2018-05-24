using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Commitments.Handlers
{
    public class StoreCommitmentHandler
    {
        private readonly IEmployerCommitmentRepository _repository;
        private readonly ILog _logger;

        public StoreCommitmentHandler(IEmployerCommitmentRepository repository, ILog logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PaymentCreatedMessage message)
        {
            if (message.EarningDetails == null)
                throw new InvalidOperationException($"Invalid payment created message. Earning details is null so cannot create commitment data. Employer account: {message.EmployerAccountId}, payment id: {message.Id}");

            var employerCommitment = await _repository.Get(message.EmployerAccountId, message.ApprenticeshipId);
            
            employerCommitment.RegisterCommitment(Map(message));

            _logger.Debug($"Now storing the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.Id}");
            await _repository.Store(employerCommitment);
            _logger.Info($"Finished adding the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.Id}");
        }

        private CommitmentModel Map(PaymentCreatedMessage message)
        {
            return new CommitmentModel
            {
                ApprenticeName = message.ApprenticeName,
                SendingEmployerAccountId = message.SendingEmployerAccountId,
                FundingSource = message.FundingSource,
                LearnerId = message.ApprenticeshipId,
                CourseLevel = message.CourseLevel,
                CourseName = message.CourseName,
                ProviderId = message.Ukprn,
                ProviderName = message.ProviderName,
                StartDate = message.EarningDetails.StartDate,
                PlannedEndDate = message.EarningDetails.PlannedEndDate,
                ActualEndDate = message.EarningDetails.ActualEndDate,
                MonthlyInstallment = message.EarningDetails.MonthlyInstallment,
                CompletionAmount = message.EarningDetails.CompletionAmount,
                NumberOfInstallments = (short)message.EarningDetails.TotalInstallments
            };
        }
    }
}