using System.Collections.Generic;
using SFA.DAS.Forecasting.Core.Validation;
using SFA.DAS.Forecasting.Levy.Application.Messages;

namespace SFA.DAS.Forecasting.Levy.Application.Validation
{
    public class LevyDeclarationEventValidator: ISuperficialValidation<LevyDeclarationEvent>
    {
        public List<ValidationFailure> Validate(LevyDeclarationEvent itemToValidate)
        {
            throw new System.NotImplementedException();
        }
    }
}