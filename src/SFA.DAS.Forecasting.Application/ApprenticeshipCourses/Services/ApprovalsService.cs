﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.NLog.Logger;
using GetApprenticeshipsResponse = SFA.DAS.Forecasting.Application.Infrastructure.OuterApi.GetApprenticeshipsResponse;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services
{
    public interface IApprovalsService
    {
        Task<List<Models.Approvals.Apprenticeship>> GetApprenticeships(long employerAccountId);
        Task<List<long>> GetEmployerAccountIds();
    }

    public class ApprovalsService : IApprovalsService
    {
        private readonly IApiClient _apiClient;
        private readonly ILog _logger;
        public const int PageSize = 1000;

        public ApprovalsService(IApiClient apiClient, ILog logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<List<Models.Approvals.Apprenticeship>> GetApprenticeships(long employerAccountId)
        {
            _logger.Info($"Getting apprenticeships for account {employerAccountId}");

            var waitingToStart = await GetApprenticeshipsForStatus(employerAccountId, GetApprenticeshipsApiRequest.ApprenticeshipStatus.WaitingToStart);
            var live = await GetApprenticeshipsForStatus(employerAccountId, GetApprenticeshipsApiRequest.ApprenticeshipStatus.Live);

            var all = waitingToStart.Union(live).ToList();

            _logger.Info($"Retrieved {all.Count} apprenticeships for account {employerAccountId}");

            return all.Select(x => new Models.Approvals.Apprenticeship
            {
                Id = x.Id,
                EmployerAccountId = employerAccountId,
                TransferSenderId = x.TransferSenderId,
                Uln = x.Uln,
                ProviderId = x.ProviderId,
                ProviderName = x.ProviderName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                CourseCode = x.CourseCode,
                CourseName = x.CourseName,
                StartDate= x.StartDate,
                EndDate = x.EndDate,
                Cost = x.Cost
            }).ToList();
        }

        private async Task<List<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>> GetApprenticeshipsForStatus(long employerAccountId, short status)
        {
            var apiRequest = new GetApprenticeshipsApiRequest(employerAccountId, status, 1, PageSize);
            var response = await _apiClient.Get<GetApprenticeshipsResponse>(apiRequest);

            var pageNumber = 2;

            var totalPages = (int)Math.Ceiling((double)response.TotalApprenticeshipsFound / PageSize);

            var apprenticeships = response.Apprenticeships.ToList();

            if (totalPages > 1)
            {
                while (pageNumber <= totalPages)
                {
                    apiRequest = new GetApprenticeshipsApiRequest(employerAccountId, status, pageNumber, PageSize);
                    response = await _apiClient.Get<GetApprenticeshipsResponse>(apiRequest);
                    apprenticeships.AddRange(response.Apprenticeships);

                    pageNumber++;
                }
            }

            return apprenticeships;
        }

        public async Task<List<long>> GetEmployerAccountIds()
        {
            var apiRequest = new GetAccountIdsApiRequest();
            var response = await _apiClient.Get<GetAccountIdsResponse>(apiRequest);
            return response.AccountIds;
        }
    }
}