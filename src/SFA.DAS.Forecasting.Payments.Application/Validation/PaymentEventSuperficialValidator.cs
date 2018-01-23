using System.Collections.Generic;
using SFA.DAS.Forecasting.Core.Validation;
using SFA.DAS.Forecasting.Payments.Application.Messages;

namespace SFA.DAS.Forecasting.Payments.Application.Validation
{
    public class PaymentEventSuperficialValidator: ISuperficialValidation<PaymentEvent>
    {
        public List<ValidationFailure> Validate(PaymentEvent itemToValidate)
        {
            throw new System.NotImplementedException();
        }
    }
}