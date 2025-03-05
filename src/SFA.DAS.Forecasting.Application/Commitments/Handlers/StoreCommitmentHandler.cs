using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Apprenticeship.Mapping;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Domain.Commitments;

namespace SFA.DAS.Forecasting.Application.Commitments.Handlers;

public interface IStoreCommitmentHandler
{
    Task Handle(PaymentCreatedMessage message, string allowProjectionsEndpoint);
    Task Handle(ApprenticeshipMessage message, string allowProjectionsEndpoint);
}
public class StoreCommitmentHandler : IStoreCommitmentHandler
{
    private readonly IPaymentMapper _paymentMapper;
    private readonly IEmployerCommitmentRepository _repository;
    private readonly ILogger<StoreCommitmentHandler> _logger;
    private readonly IQueueService _queueService;
    public StoreCommitmentHandler(
        IEmployerCommitmentRepository repository, 
        ILogger<StoreCommitmentHandler> logger, 
        IPaymentMapper paymentMapper,
        IQueueService queueService)
    {
        _repository = repository;
        _logger = logger;
        _queueService = queueService;
        _paymentMapper = paymentMapper;
    }

    public async Task Handle(PaymentCreatedMessage message, string allowProjectionsEndpoint)
    {
        if (message.EarningDetails == null)
            throw new InvalidOperationException($"Invalid payment created message. Earning details is null so cannot create commitment data. Employer account: {message.EmployerAccountId}, payment id: {message.Id}");

          
        var commitment = await _repository.Get(message.EmployerAccountId, message.ApprenticeshipId);
        var commitmentModel = _paymentMapper.MapToCommitment(message);
        if (!commitment.RegisterCommitment(commitmentModel))
        {
            _logger.LogInformation($"Not storing the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.ApprenticeshipId}, payment id: {message.Id}");
            return;
        }
            
        _logger.LogInformation($"Now storing the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.ApprenticeshipId}, payment id: {message.Id}");
        await _repository.Store(commitment);
        _logger.LogInformation($"Finished adding the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.ApprenticeshipId}, payment id: {message.Id}");
        await _queueService.SendMessageWithVisibilityDelay(message, allowProjectionsEndpoint);
    }

    public async Task Handle(ApprenticeshipMessage message, string allowProjectionsEndpoint)

    {
        if (message.LearnerId < 0)
            throw new InvalidOperationException("Apprenticeship requires LearnerId");
        if (message.CourseLevel <= 0)
            throw new InvalidOperationException("Apprenticeship requires CourseLevel");

        var commitment = await _repository.Get(message.EmployerAccountId, message.ApprenticeshipId);
        var commitmentModel = ApprenticeshipMapping.MapToCommitment(message);
        if (!commitment.RegisterCommitment(commitmentModel))
        {
            _logger.LogInformation($"Not storing the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.ApprenticeshipId} ");
            return;
        }

        await _repository.Store(commitment);

        _logger.LogInformation($"Finished adding the employer commitment. Employer: {message.EmployerAccountId}, ApprenticeshipId: {message.ApprenticeshipId}");
        await _queueService.SendMessageWithVisibilityDelay(message, allowProjectionsEndpoint);
    }
}