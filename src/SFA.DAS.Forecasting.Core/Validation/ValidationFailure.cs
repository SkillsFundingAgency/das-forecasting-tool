using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Core.Validation
{
    public class ValidationFailure
    {
        public string Reason { get; set; }
    }

    public class ValidationFailure<T> where T: class
    {
        public T FailedItem { get; }
        public List<string> Reasons { get; }

        public ValidationFailure(List<string> reasons, T failedItem)
        {
            Reasons = reasons ?? throw new ArgumentNullException(nameof(reasons));
            FailedItem = failedItem ?? throw new ArgumentNullException(nameof(failedItem));
        }
    }
}