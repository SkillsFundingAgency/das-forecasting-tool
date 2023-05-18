using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.Models.Approvals;

namespace SFA.DAS.Forecasting.Commitments.Functions.Application
{
    internal class ApprenticeshipValidation
    {
        internal Tuple<List<Apprenticeship>, IEnumerable<long>> BusinessValidation(IList<Apprenticeship> apprenticeships)
        {
            var filteredApprenticeships =
                apprenticeships
                    .Where(m => !m.HasHadDataLockSuccess).ToList();

            return CreateResult(filteredApprenticeships, apprenticeships);
        }

        private Tuple<List<Apprenticeship>, IEnumerable<long>> CreateResult(List<Apprenticeship> filteredApprenticeships, IList<Apprenticeship> apprenticeships)
        {
            var failedValidation = apprenticeships
                        .Select(m => m.Id).Except(filteredApprenticeships.Select(m => m.Id));

            return Tuple.Create(filteredApprenticeships, failedValidation);
        }
    }
}
