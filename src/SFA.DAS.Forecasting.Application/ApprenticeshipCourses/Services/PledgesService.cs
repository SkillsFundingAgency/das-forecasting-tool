using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Models.Pledges;

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
        private readonly ILogger<PledgesService> _logger;

        public PledgesService(IApiClient apiClient, ILogger<PledgesService> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<List<Pledge>> GetPledges()
        {
            try
            {
                var response = await _apiClient.Get<GetPledgesResponse>(new GetPledgesApiRequest());
                _logger.LogInformation($"LTM inner api reports {response.TotalPledges} total pledges");
                return new List<Pledge>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred getting pledges");
                throw;
            }
        }

        public async Task<List<Models.Pledges.Application>> GetApplications()
        {
            var response = await _apiClient.Get<GetApplicationsResponse>(new GetApplicationsApiRequest());
            _logger.LogInformation($"LTM inner api reports {response.TotalApplications} total applications");
            return new List<Models.Pledges.Application>();
        }
    }
}
