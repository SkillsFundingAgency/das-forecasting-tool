using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public class GetApprenticeshipsApiRequest : IGetApiRequest
    {
        public long EmployerAccountId { get; }
        public int Status { get; }
        public int Page { get; }
        private readonly int _pageItemCount;

        public GetApprenticeshipsApiRequest(long employerAccountId, short status, int page, int pageItemCount)
        {
            EmployerAccountId = employerAccountId;
            Status = status;
            Page = page;
            _pageItemCount = pageItemCount;
        }

        public string GetUrl => $"approvals/apprenticeships?accountId={EmployerAccountId}&status={Status}&page={Page}&pageItemCount={_pageItemCount}";

        public static class ApprenticeshipStatus
        {
            public const short WaitingToStart = 0;
            public const short Live = 1;
        }
    }

    public class GetApprenticeshipsResponse
    {
        public IEnumerable<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse> Apprenticeships { get; set; }

        public int TotalApprenticeshipsFound { get; set; }

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
            public int CourseLevel { get; set; }

            public DateTime StartDate { get; set; }

            public DateTime EndDate { get; set; }

            public long? TransferSenderId { get; set; }

            public bool HasHadDataLockSuccess { get; set; }

            public string CourseCode { get; set; }

            public Decimal? Cost { get; set; }
            public int? PledgeApplicationId { get; set; }
        }
    }
}
