using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Domain.Payments;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public class ProcessEmployerPaymentHandler
    {
        private readonly IEmployerPaymentRepository _repository;
        private readonly ILogger<ProcessEmployerPaymentHandler> _logger;
        private readonly IPaymentMapper _mapper;

        public ProcessEmployerPaymentHandler(IEmployerPaymentRepository repository, ILogger<ProcessEmployerPaymentHandler> logger, IPaymentMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task Handle(PaymentCreatedMessage paymentCreatedMessage, string allowProjectionsEndpoint)
        {
            var employerPayment = _mapper.MapToPayment(paymentCreatedMessage);
            _logger.LogDebug($"Now storing the employer payment. Employer: {employerPayment.EmployerAccountId}, Payment Id: {employerPayment.ExternalPaymentId}, Collection period: {employerPayment.CollectionPeriod.Year} - {employerPayment.CollectionPeriod.Month}, Delivery period: {employerPayment.DeliveryPeriod.Year} - {employerPayment.DeliveryPeriod.Month}");
            var payment = await _repository.Get(paymentCreatedMessage.EmployerAccountId, paymentCreatedMessage.Id);
            payment.RegisterPayment(employerPayment);
            await _repository.StorePayment(payment);
            _logger.LogInformation($"Finished adding the employer payment. Employer: {employerPayment.EmployerAccountId}, Payment Id: {employerPayment.ExternalPaymentId}, Collection period: {employerPayment.CollectionPeriod.Year} - {employerPayment.CollectionPeriod.Month}, Delivery period: {employerPayment.DeliveryPeriod.Year} - {employerPayment.DeliveryPeriod.Month}");

        }
    }
}