using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.Forecasting.Commitments.Functions.Application
{
    internal class ApprenticeshipValidation
    {
        internal Tuple<List<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>, IEnumerable<long>> BusinessValidation(IList<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse> apprenticeships)
        {
            var filteredApprenticeships =
                apprenticeships
                .Where(m => !m.HasHadDataLockSuccess)
                .Where(m => m.ApprenticeshipStatus == CommitmentsV2.Types.ApprenticeshipStatus.Live || m.ApprenticeshipStatus == CommitmentsV2.Types.ApprenticeshipStatus.WaitingToStart).ToList();

            return CreateResult(filteredApprenticeships, apprenticeships);
        }

        private Tuple<List<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>, IEnumerable<long>> CreateResult(
            List<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse> filteredApprenticeships,
            IList<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse> apprenticeships)
        {
            var failedValidation = apprenticeships
                        .Select(m => m.Id).Except(filteredApprenticeships.Select(m => m.Id));

            return Tuple.Create(filteredApprenticeships, failedValidation);
        }
    }
}
