using System.Collections.Generic;
using SFA.DAS.Forecasting.Core.Validation;
using SFA.DAS.Forecasting.Payments.Application.Messages;

namespace SFA.DAS.Forecasting.Payments.Application.Validation
{
    public class EarningDetailsSuperficialValidator : ISuperficialValidation<EarningDetails>
    {
        public List<ValidationFailure> Validate(EarningDetails itemToValidate)
        {
            throw new System.NotImplementedException();
        }
    }
}