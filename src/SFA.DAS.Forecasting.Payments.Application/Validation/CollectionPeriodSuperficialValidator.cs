using System.Collections.Generic;
using SFA.DAS.Forecasting.Core.Validation;
using SFA.DAS.Forecasting.Payments.Application.Messages;

namespace SFA.DAS.Forecasting.Payments.Application.Validation
{
    public class CollectionPeriodSuperficialValidator : ISuperficialValidation<CollectionPeriod>
    {
        public List<ValidationFailure> Validate(CollectionPeriod itemToValidate)
        {
            throw new System.NotImplementedException();
        }
    }
}