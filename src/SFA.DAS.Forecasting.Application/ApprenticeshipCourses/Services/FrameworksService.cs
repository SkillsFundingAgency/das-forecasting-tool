using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
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

        public FrameworksService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<ApprenticeshipCourse>> GetCourses()
        {
            var response = await _apiClient.Get<ApprenticeshipCourseFrameworkResponse>(new GetFrameworksApiRequest());
            
            return response.Frameworks.Select(c => c).ToList();
        }
    }
}