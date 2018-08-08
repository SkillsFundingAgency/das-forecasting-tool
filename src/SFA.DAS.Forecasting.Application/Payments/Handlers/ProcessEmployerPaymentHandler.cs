using System;
using System.Diagnostics;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
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
        private readonly ITelemetry _telemetry;

        public ProcessEmployerPaymentHandler(IEmployerPaymentRepository repository, ILog logger, IPaymentMapper mapper,
            IApplicationConfiguration configuration, IQueueService queueService, ITelemetry telemetry)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _queueService = queueService ?? throw new ArgumentNullException(nameof(queueService));
            _telemetry = telemetry ?? throw new ArgumentNullException(nameof(telemetry));
        }

        public async Task Handle(PaymentCreatedMessage paymentCreatedMessage, string allowProjectionsEndpoint)
        {
            _telemetry.AddEmployerAccountId(paymentCreatedMessage.EmployerAccountId);
            _telemetry.AddProperty("Payment Id",paymentCreatedMessage.Id);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var employerPayment = _mapper.MapToPayment(paymentCreatedMessage);
            _logger.Debug($"Now storing the employer payment. Employer: {employerPayment.EmployerAccountId}, Payment Id: {employerPayment.ExternalPaymentId}, Collection period: {employerPayment.CollectionPeriod.Year} - {employerPayment.CollectionPeriod.Month}, Delivery period: {employerPayment.DeliveryPeriod.Year} - {employerPayment.DeliveryPeriod.Month}");
            var payment = await _repository.Get(paymentCreatedMessage.EmployerAccountId, paymentCreatedMessage.Id);
            payment.RegisterPayment(employerPayment);
            await _repository.StorePayment(payment);
            _logger.Info($"Finished adding the employer payment. Employer: {employerPayment.EmployerAccountId}, Payment Id: {employerPayment.ExternalPaymentId}, Collection period: {employerPayment.CollectionPeriod.Year} - {employerPayment.CollectionPeriod.Month}, Delivery period: {employerPayment.DeliveryPeriod.Year} - {employerPayment.DeliveryPeriod.Month}");
            _queueService.SendMessageWithVisibilityDelay(paymentCreatedMessage, allowProjectionsEndpoint);
            stopwatch.Stop();
            _telemetry.TrackDuration("Store Payment", stopwatch.Elapsed);
            _telemetry.TrackEvent("Stored Payment");
        }
    }
}