using System.Collections.Generic;
using System.Xml.Schema;

namespace SFA.DAS.Forecasting.Core.Validation
{
    public interface ISuperficialValidation<in T>
    {
        List<ValidationFailure> Validate(T itemToValidate);
    }
}