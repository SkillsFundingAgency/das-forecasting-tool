using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Models.Pledges;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services
{
    public interface IPledgesService
    {
        Task<List<long>> GetAccountIds();
        Task<List<Pledge>> GetPledges(long accountId);
        Task<List<Models.Pledges.Application>> GetApplications(int pledgeId);
    }

    public class PledgesService : IPledgesService
    {
        private readonly IApiClient _apiClient;
        private readonly ILog _logger;

        public PledgesService(IApiClient apiClient, ILog logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<List<long>> GetAccountIds()
        {
            var response = await _apiClient.Get<GetPledgeAccountIdsResponse>(new GetPledgeAccountIdsApiRequest());
            return response.AccountIds;
        }

        public async Task<List<Pledge>> GetPledges(long accountId)
        {
            try
            {
                var request = new GetPledgesApiRequest(accountId);
                var response = await _apiClient.Get<GetPledgesResponse>(request);
                _logger.Info($"LTM inner api reports {response.TotalPledges} total pledges for account {accountId}");

                return response.Pledges.Select(x => new Pledge { AccountId = x.AccountId }).ToList();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting pledges");
                throw;
            }
        }

        public async Task<List<Models.Pledges.Application>> GetApplications(int pledgeId)
        {
            var request = new GetApplicationsApiRequest(pledgeId);
            var response = await _apiClient.Get<GetApplicationsResponse>(request);
            _logger.Info($"LTM inner api reports {response.Applications.Count} applications for pledge {pledgeId}");
            return new List<Models.Pledges.Application>();
        }
    }
}
