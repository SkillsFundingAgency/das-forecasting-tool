using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.ApprenticeshipCourses;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services
{
    public interface IFrameworksService
    {
        Task<List<ApprenticeshipCourse>> GetCourses();
    }

    public class FrameworksService : IFrameworksService
    {
        private readonly IApiClient _apiClient;
        private readonly IApplicationConfiguration _config;

        public FrameworksService(IApiClient apiClient, IApplicationConfiguration config)
        {
            _apiClient = apiClient;
            _config = config;
        }

        public async Task<List<ApprenticeshipCourse>> GetCourses()
        {
            var request = new GetFrameworksApiRequest(_config.ApprenticeshipsApiBaseUri);
            var response = await _apiClient.Get<List<ApprenticeshipCourse>>(request);
            
            return response.Select(c => c).Where(f => f.CourseType == ApprenticeshipCourseType.Framework).ToList();
        }
    }
}