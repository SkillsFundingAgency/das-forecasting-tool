using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.NLog.Logger;

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

        public ApprovalsService(IApiClient apiClient, ILog logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<List<Models.Approvals.Apprenticeship>> GetApprenticeships(long employerAccountId)
        {
            _logger.Info($"Getting apprenticeships for account {employerAccountId}");

            try
            {
                _logger.Info($"Base url: {_apiClient.BaseUrl}");
                var apiRequest = new GetApprenticeshipsApiRequest(employerAccountId, 0, 1, 100);
                _logger.Info($"Getting: {apiRequest.GetUrl}");
                var response = await _apiClient.Get<GetApprenticeshipsResponse>(apiRequest);
                _logger.Info($"Api reports {response.TotalApprenticeships} apprenticeships for account {employerAccountId}");
                return new List<Models.Approvals.Apprenticeship>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting apprenticeships");
                throw;
            }
        }

        public async Task<List<long>> GetEmployerAccountIds()
        {
            var apiRequest = new GetAccountIdsApiRequest();
            var response = await _apiClient.Get<GetAccountIdsResponse>(apiRequest);
            return response.AccountIds;
        }
    }
}
