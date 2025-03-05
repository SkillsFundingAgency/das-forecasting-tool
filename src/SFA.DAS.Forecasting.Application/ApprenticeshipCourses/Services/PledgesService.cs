using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
using SFA.DAS.Forecasting.Models.Pledges;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;

public interface IPledgesService
{
    Task<List<long>> GetAccountIds();
    Task<List<Pledge>> GetPledges(long accountId);
    Task<List<Models.Pledges.Application>> GetApplications(int pledgeId);
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

    public async Task<List<long>> GetAccountIds()
    {
        try
        {
            var request = new GetPledgeAccountIdsApiRequest();

            var response = await _apiClient.Get<GetPledgeAccountIdsResponse>(request);
            return response.AccountIds;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting account Ids");
            throw;
        }
    }

    public async Task<List<Pledge>> GetPledges(long accountId)
    {
        try
        {
            var request = new GetPledgesApiRequest(accountId);
            var response = await _apiClient.Get<GetPledgesResponse>(request);
            _logger.LogInformation($"LTM inner api reports {response.Pledges.Count} total pledges for account {accountId}");

            return response.Pledges.Select(x => new Pledge { AccountId = x.AccountId, Id = x.Id }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting pledges");
            throw;
        }
    }

    public async Task<List<Models.Pledges.Application>> GetApplications(int pledgeId)
    {
        var request = new GetApplicationsApiRequest(pledgeId);
        var response = await _apiClient.Get<GetApplicationsResponse>(request);
        _logger.LogInformation($"LTM inner api reports {response.Applications.Count} applications for pledge {pledgeId}");

        return response.Applications.Select(x => new Models.Pledges.Application
        {
            Id = x.Id,
            EmployerAccountId = x.EmployerAccountId,
            PledgeId = x.PledgeId,
            StandardId = x.StandardId,
            StandardTitle = x.StandardTitle,
            StandardLevel = x.StandardLevel,
            StandardDuration = x.StandardDuration,
            StandardMaxFunding = x.StandardMaxFunding,
            StartDate = x.StartDate,
            NumberOfApprentices = x.NumberOfApprentices,
            NumberOfApprenticesUsed = x.NumberOfApprenticesUsed,
            Status = x.Status
        }).ToList();
    }
}