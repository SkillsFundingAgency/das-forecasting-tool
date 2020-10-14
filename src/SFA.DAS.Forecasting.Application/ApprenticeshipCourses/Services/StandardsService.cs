using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.ApprenticeshipCourses;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services
{
    public interface IStandardsService
    {
        Task<List<ApprenticeshipCourse>> GetCourses();
    }

    public class StandardsService: IStandardsService
    {
        private readonly IApiClient _apiClient;
        private readonly ApplicationConfiguration _config;

        public StandardsService(IApiClient apiClient, ApplicationConfiguration config)
        {
            _apiClient = apiClient;
            _config = config;
        }

        public async Task<List<ApprenticeshipCourse>> GetCourses()
        {
            var request = new GetStandardsApiRequest(_config.ApprenticeshipsApiBaseUri);
            var response = await _apiClient.Get<List<ApprenticeshipCourse>>(request);

            return response.Select(c => c).Where(s => s.CourseType == ApprenticeshipCourseType.Standard).ToList();
        }

    }
}