using SFA.DAS.Forecasting.Payments.Messages.Events;
using System;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Payments.Domain.Apprenticeships
{
    public interface IApprenticeshipService
    {
        Task AddPayment(PaymentEvent paymentEvent);
    }

    public class ApprenticeshipService : IApprenticeshipService
    {
        public IApprenticeshipRepository Repository { get; }
        public IPaymentEventMapper Mapper { get; }

        public ApprenticeshipService(IApprenticeshipRepository repository, IPaymentEventMapper mapper)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task AddPayment(PaymentEvent paymentEvent)
        {
            //record add payment
            var payment = Mapper.Map(paymentEvent);
            await Repository.InsertOrUpdatePayment(payment);
        }
    }
}