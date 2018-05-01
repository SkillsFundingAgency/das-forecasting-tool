using System;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public class ProcessEmployerPaymentHandler
    {
        private readonly IEmployerPaymentRepository _repository;
        private readonly ILog _logger;
        private readonly IPaymentMapper _mapper;
        private readonly IApplicationConfiguration _configuration;
        private readonly IQueueService _queueService;

        public ProcessEmployerPaymentHandler(IEmployerPaymentRepository repository, ILog logger, IPaymentMapper mapper, 
            IApplicationConfiguration configuration, IQueueService queueService)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
        }

        public async Task Handle(PaymentCreatedMessage paymentCreatedMessage, string allowProjectionsEndpoint)
        {
	        var employerPayment = _mapper.MapToPayment(paymentCreatedMessage);
			_logger.Debug($"Now storing the employer payment. Employer: {employerPayment.EmployerAccountId}, Payment Id: {employerPayment.ExternalPaymentId}, Collection period: {employerPayment.CollectionPeriod.Year} - {employerPayment.CollectionPeriod.Month}, Delivery period: {employerPayment.DeliveryPeriod.Year} - {employerPayment.DeliveryPeriod.Month}");
            var payment = await _repository.Get(paymentCreatedMessage.EmployerAccountId, paymentCreatedMessage.Id);
            payment.RegisterPayment(employerPayment);
			await _repository.StorePayment(payment);
            _logger.Info($"Finished adding the employer payment. Employer: {employerPayment.EmployerAccountId}, Payment Id: {employerPayment.ExternalPaymentId}, Collection period: {employerPayment.CollectionPeriod.Year} - {employerPayment.CollectionPeriod.Month}, Delivery period: {employerPayment.DeliveryPeriod.Year} - {employerPayment.DeliveryPeriod.Month}");
            _queueService.SendMessageWithVisibilityDelay(paymentCreatedMessage, allowProjectionsEndpoint, TimeSpan.FromSeconds(_configuration.SecondsToWaitToAllowProjections));
        }
    }
}