using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services
{
    public interface IFrameworksService
    {
        Task<List<ApprenticeshipCourse>> GetCourses();
    }

    public class FrameworksService : IFrameworksService
    {
        private readonly IFrameworkApiClient _frameworkApiClient;
        private readonly IApprenticehipsCourseMapper _mapper;

        public FrameworksService(IFrameworkApiClient frameworkApiClient, IApprenticehipsCourseMapper mapper)
        {
            _frameworkApiClient = frameworkApiClient;
            _mapper = mapper;
        }

        public async Task<List<ApprenticeshipCourse>> GetCourses()
        {
            var frameworks = await _frameworkApiClient.GetAllAsync();

            return frameworks
                .Where(course => course.IsActiveFramework)
                .Where(c => c.FundingPeriods != null)
                .Select(_mapper.Map)
                .ToList();
        }
    }
}