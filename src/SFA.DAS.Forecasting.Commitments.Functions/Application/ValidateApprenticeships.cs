using System;
using System.Collections.Generic;
using System.Linq;
using ApiApprenticeship = SFA.DAS.Commitments.Api.Types.Apprenticeship.Apprenticeship;

namespace SFA.DAS.Forecasting.Commitments.Functions.Application
{
    internal class ApprenticeshipValidation
    {
        internal Tuple<List<ApiApprenticeship>, IEnumerable<long>> BusinessValidation(IList<ApiApprenticeship> apprenticeships)
        {
            var filteredApprenticeships =
                apprenticeships
                .Where(m => !m.HasHadDataLockSuccess)
                .Where(m => m.StopDate == null)
                .Where(m => m.PauseDate == null)
                .Where(m => m.PaymentStatus == DAS.Commitments.Api.Types.Apprenticeship.Types.PaymentStatus.Active).ToList();

            return CreateResult(filteredApprenticeships, apprenticeships);
        }

        internal Tuple<List<ApiApprenticeship>, IEnumerable<long>> InputValidation(IList<ApiApprenticeship> apprenticeships)
        {
            var filteredApprenticeships =
                apprenticeships
                .Where(m => m.StartDate.HasValue)
                .Where(m => m.EndDate.HasValue)
                .Where(m => m.Cost.HasValue).ToList();

            return CreateResult(filteredApprenticeships, apprenticeships);
        }

        private Tuple<List<ApiApprenticeship>, IEnumerable<long>> CreateResult(
            List<ApiApprenticeship> filteredApprenticeships,
            IList<ApiApprenticeship> apprenticeships)
        {
            var failedValidation = apprenticeships
                        .Select(m => m.Id).Except(filteredApprenticeships.Select(m => m.Id));

            return Tuple.Create(filteredApprenticeships, failedValidation);
        }
    }
}
