using System.Collections.Generic;

using SFA.DAS.Forecasting.Domain.Entities;

namespace SFA.DAS.Forecasting.Application.Queries.Apprenticeships
{
    public class ApprenticeshipsResponse
    {
        public IEnumerable<Apprenticeship> Data { get; set; }
    }
}