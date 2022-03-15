using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class GetApprenticeshipsApiRequest : IGetApiRequest
    {
        public string GetUrl => "apprenticeships";
    }

    public class GetApprenticeshipsResponse
    {
        public IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse> Apprenticeships { get; set; }

        public int TotalApprenticeshipsFound { get; set; }

        public int TotalApprenticeshipsWithAlertsFound { get; set; }

        public int TotalApprenticeships { get; set; }

        public int PageNumber { get; set; }

        public class ApprenticeshipDetailsResponse
        {
            public long Id { get; set; }

            public string FirstName { get; set; }

            public string LastName { get; set; }

            public string Email { get; set; }

            public string Uln { get; set; }

            public string EmployerName { get; set; }

            public string ProviderName { get; set; }

            public long ProviderId { get; set; }

            public string CourseName { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public DateTime PauseDate { get; set; }

            public DateTime DateOfBirth { get; set; }

            public Decimal? TotalAgreedPrice { get; set; }

            public string EmployerRef { get; set; }

            public string ProviderRef { get; set; }

            public string CohortReference { get; set; }

            public long AccountLegalEntityId { get; set; }

            public long? TransferSenderId { get; set; }

            public bool HasHadDataLockSuccess { get; set; }

            public string CourseCode { get; set; }

            public Decimal? Cost { get; set; }
        }
    }
}
