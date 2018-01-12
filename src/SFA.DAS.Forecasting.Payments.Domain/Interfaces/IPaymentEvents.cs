using SFA.DAS.Forecasting.Payments.Messages.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Payments.Domain.Interfaces
{
    public interface IPaymentEvents
    {
        Task<IEnumerable<PaymentMessage>> ReadAsync();
    }
}
