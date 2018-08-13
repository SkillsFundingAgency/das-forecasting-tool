using System;
using System.Collections.Generic;
using System.Linq;

using ApiApprenticeship = SFA.DAS.Commitments.Api.Types.Apprenticeship.Apprenticeship;

namespace SFA.DAS.Forecasting.Apprenticeship.Functions.Application
{
    internal class ApprenticeshipValidation
    {
        internal Tuple<IEnumerable<ApiApprenticeship>, IEnumerable<long>> BusinessValidation(IEnumerable<ApiApprenticeship> apprenticeships)
        {
            var filteredApprenticeships =
                apprenticeships
                .Where(m => !m.HasHadDataLockSuccess)
                .Where(m => m.StopDate == null)
                .Where(m => m.PauseDate == null)
                .Where(m => m.PaymentStatus == Commitments.Api.Types.Apprenticeship.Types.PaymentStatus.Active);

            return CreateResult(filteredApprenticeships, apprenticeships);
        }

        internal Tuple<IEnumerable<ApiApprenticeship>, IEnumerable<long>> InputValidation(IEnumerable<ApiApprenticeship> apprenticeships)
        {
            var filteredApprenticeships =
                apprenticeships
                .Where(m => m.StartDate.HasValue)
                .Where(m => m.EndDate.HasValue)
                .Where(m => m.Cost.HasValue);

            return CreateResult(filteredApprenticeships, apprenticeships);
        }

        private Tuple<IEnumerable<ApiApprenticeship>, IEnumerable<long>> CreateResult(
            IEnumerable<ApiApprenticeship> filteredApprenticeships,
            IEnumerable<ApiApprenticeship> apprenticeships)
        {
            var failedValidation = apprenticeships
                        .Select(m => m.Id).Except(filteredApprenticeships.Select(m => m.Id));

            return Tuple.Create(filteredApprenticeships, failedValidation);
        }
    }
}
