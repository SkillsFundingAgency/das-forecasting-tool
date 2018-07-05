using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services
{
    public interface IStandardsService
    {
        Task<List<ApprenticeshipCourse>> GetCourses();
    }

    public class StandardsService: IStandardsService
    {
	    private readonly IAppInsightsTelemetry _appInsightsTelemetry;
	    private readonly IStandardApiClient _standardApiClient;
        private readonly IStandardSummaryMapper _mapper;

        public StandardsService(IStandardApiClient standardApiClient, IStandardSummaryMapper mapper, IAppInsightsTelemetry appInsightsTelemetry)
        {
	        _appInsightsTelemetry = appInsightsTelemetry;
	        _standardApiClient = standardApiClient ?? throw new ArgumentNullException(nameof(standardApiClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ApprenticeshipCourse>> GetCourses()
        {
            var standards = (await _standardApiClient.GetAllAsync()).ToList();
            var response = standards.Where(course => course.IsActiveStandard).Select(_mapper.Map).ToList();
			_appInsightsTelemetry.Info("GetStandardsFunction", $"Got {standards.Count} standards, but only {response.Count} were active", "GetCourses");
	        return response;
        }

    }
}