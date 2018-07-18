using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Commitments.Handlers
{
    public class StoreCommitmentHandler
    {
        private readonly IPaymentMapper _paymentMapper;
        private readonly IEmployerCommitmentRepository _repository;
        private readonly IAppInsightsTelemetry _logger;

        public StoreCommitmentHandler(IEmployerCommitmentRepository repository, IAppInsightsTelemetry logger, IPaymentMapper paymentMapper)
        {
            _paymentMapper = paymentMapper;
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PaymentCreatedMessage message)
        {
	        if (message.EarningDetails == null)
	        {
		        _logger.Error("PaymentEventStoreCommitmentFunction", 
					new InvalidOperationException($"Invalid payment created message. Earning details is null so cannot create commitment data. Employer account: {message.EmployerAccountId}, payment id: {message.Id}"),
			        $"Invalid payment created message. Earning details is null so cannot create commitment data. Employer account: {message.EmployerAccountId}, payment id: {message.Id}", 
					"FunctionRunner.Run");

				throw new InvalidOperationException(
			        $"Invalid payment created message. Earning details is null so cannot create commitment data. Employer account: {message.EmployerAccountId}, payment id: {message.Id}");
	        }
	        var employerCommitment = await _repository.Get(message.EmployerAccountId, message.ApprenticeshipId);

			var commitmentModel = _paymentMapper.MapToCommitment(message);
            
            var commitmentRegistered = employerCommitment.RegisterCommitment(commitmentModel);
            if (!commitmentRegistered)
            {
	            _logger.Debug("PaymentEventStoreCommitmentFunction", $"Not storing the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.Id}", "Handle");

                return;
            }

	        _logger.Debug("PaymentEventStoreCommitmentFunction", $"Now storing the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.Id}", "Handle");

            await _repository.Store(employerCommitment);
	        _logger.Info("PaymentEventStoreCommitmentFunction", $"Finished adding the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.Id}", "Handle");
        }

    }
}