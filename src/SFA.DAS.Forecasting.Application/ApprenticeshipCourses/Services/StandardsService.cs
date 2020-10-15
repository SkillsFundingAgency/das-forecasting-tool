using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Application.Infrastructure.OuterApi;
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

        public StandardsService(IApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<List<ApprenticeshipCourse>> GetCourses()
        {
            var response = await _apiClient.Get<List<ApprenticeshipCourse>>(new GetStandardsApiRequest());

            return response.Select(c => c).ToList();
        }

    }
}