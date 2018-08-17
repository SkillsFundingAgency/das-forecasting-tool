using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services
{
    public interface IStandardsService
    {
        Task<List<ApprenticeshipCourse>> GetCourses();
    }

    public class StandardsService: IStandardsService
    {
        private readonly IStandardApiClient _standardApiClient;
        private readonly IApprenticehipsCourseMapper _mapper;

        public StandardsService(IStandardApiClient standardApiClient, IApprenticehipsCourseMapper mapper)
        {
            _standardApiClient = standardApiClient ?? throw new ArgumentNullException(nameof(standardApiClient));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<ApprenticeshipCourse>> GetCourses()
        {
            var standards = (await _standardApiClient.GetAllAsync()).ToList();
            return standards.Where(course => course.IsActiveStandard).Select(_mapper.Map).ToList();
        }

    }
}