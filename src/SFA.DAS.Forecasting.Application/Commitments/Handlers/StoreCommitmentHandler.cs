using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Apprenticeship.Mapping;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Commitments.Handlers
{
    public class StoreCommitmentHandler
    {
        private readonly IPaymentMapper _paymentMapper;
        private readonly ApprenticeshipMapping _apprenticeshipMapping;
        private readonly IEmployerCommitmentRepository _repository;
        private readonly ILog _logger;
        private readonly ITelemetry _telemetry;
        private readonly IQueueService _queueService;


        public StoreCommitmentHandler(
            IEmployerCommitmentRepository repository, 
            ILog logger, 
            IPaymentMapper paymentMapper,
            ITelemetry telemetry,
            ApprenticeshipMapping apprenticeshipMapping,
            IQueueService queueService)
        {
            _apprenticeshipMapping = apprenticeshipMapping;
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
            _telemetry = telemetry;
            _apprenticeshipMapping = apprenticeshipMapping;
            _paymentMapper = paymentMapper;
        }

        public async Task Handle(PaymentCreatedMessage message, string allowProjectionsEndpoint)
        {
            if (message.EarningDetails == null)
                throw new InvalidOperationException($"Invalid payment created message. Earning details is null so cannot create commitment data. Employer account: {message.EmployerAccountId}, payment id: {message.Id}");

            _telemetry.AddEmployerAccountId(message.EmployerAccountId);
            _telemetry.AddProperty("Payment Id", message.Id);
            _telemetry.AddProperty("Apprenticeship Id", message.ApprenticeshipId.ToString());
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var commitment = await _repository.Get(message.EmployerAccountId, message.ApprenticeshipId);
            var commitmentModel = _paymentMapper.MapToCommitment(message);
            if (!commitment.RegisterCommitment(commitmentModel))
            {
                _logger.Info($"Not storing the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.ApprenticeshipId}, payment id: {message.Id}");
                return;
            }
            
            _logger.Info($"Now storing the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.ApprenticeshipId}, payment id: {message.Id}");
            await _repository.Store(commitment);
            _logger.Info($"Finished adding the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.ApprenticeshipId}, payment id: {message.Id}");
            _queueService.SendMessageWithVisibilityDelay(message, allowProjectionsEndpoint);
            stopwatch.Stop();

            _telemetry.TrackDuration("Stored Commitment", stopwatch.Elapsed);
        }

        public async Task Handle(ApprenticeshipMessage message, string allowProjectionsEndpoint)

        {
            if (message.LearnerId <= 0)
                throw new InvalidOperationException("Apprenticeship requires LearnerId");
            if (message.CourseLevel <= 0)
                throw new InvalidOperationException("Apprenticeship requires CourseLevel");

            var commitment = await _repository.Get(message.EmployerAccountId, message.ApprenticeshipId);
            var commitmentModel = _apprenticeshipMapping.MapToCommitment(message);
            if (!commitment.RegisterCommitment(commitmentModel))
            {
                _logger.Info($"Not storing the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.ApprenticeshipId} ");
                return;
            }

            await _repository.Store(commitment);

            _logger.Info($"Finished adding the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.ApprenticeshipId}");
            _queueService.SendMessageWithVisibilityDelay(message, allowProjectionsEndpoint);
        }
    }
}