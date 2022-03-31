using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Models.Pledges;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services
{
    public interface IPledgesService
    {
        Task<List<Pledge>> GetPledges();
        Task<List<Models.Pledges.Application>> GetApplications();
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

        public async Task<List<Pledge>> GetPledges()
        {
            try
            {
                _logger.Info($"Getting pledges");
                var response = await _apiClient.Get<GetPledgesResponse>(new GetPledgesApiRequest());
                _logger.Info($"LTM inner api reports {response.TotalPledges} total pledges");
                return new List<Pledge>();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting pledges");
                throw;
            }
        }

        public async Task<List<Models.Pledges.Application>> GetApplications()
        {
            var response = await _apiClient.Get<GetApplicationsResponse>(new GetApplicationsApiRequest());
            _logger.Info($"LTM inner api reports {response.TotalApplications} total applications");
            return new List<Models.Pledges.Application>();
        }
    }
}
