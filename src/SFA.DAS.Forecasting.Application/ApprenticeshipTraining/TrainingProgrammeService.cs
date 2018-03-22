using SFA.DAS.Apprenticeships.Api.Client;
using SFA.DAS.Apprenticeships.Api.Types;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Shared.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipTraining
{
    public class TrainingProgrammeService
    {
        private readonly ApprenticeshipApiConfig _configuration;
        private readonly ICache _cache;

        public TrainingProgrammeService(ApprenticeshipApiConfig configuration, ICache cache)
        {
            _configuration = configuration;
            _cache = cache;
        }

        public async Task<IEnumerable<ITrainingProgramme>> GetAllActiveStandards()
        {
            return await GetStandards();            
        }

        public async Task<IEnumerable<ITrainingProgramme>> GetAllActiveCourses()
        {
            var standards = await GetStandards();
            var frameworks = await GetFrameworks();

            return standards
                .Union(frameworks)
                .OrderBy(m => m.Title);
        }

        private async Task<IEnumerable<ITrainingProgramme>> GetStandards(bool refreshCache = false)
        {
            if (!await _cache.Exists(_configuration.StandardsKey) || refreshCache)
            {
                var api = new StandardApiClient(_configuration.ApprenticeshipApiBaseUrl);

                var standards = api.FindAll().OrderBy(x => x.Title).ToList();

                await _cache.Set(_configuration.StandardsKey, standards.Select(Map));
            }

            return await _cache.Get<IEnumerable<TrainingProgramme>>(_configuration.StandardsKey);
        }

        private async Task<IEnumerable<ITrainingProgramme>> GetFrameworks(bool refreshCache = false)
        {
            if (!await _cache.Exists(_configuration.FrameworksKey) || refreshCache)
            {
                var api = new FrameworkApiClient(_configuration.ApprenticeshipApiBaseUrl);

                var frameworks = api.FindAll().OrderBy(x => x.Title).ToList();

                await _cache.Set(_configuration.FrameworksKey, frameworks.Select(Map));
            }

            return await _cache.Get<IEnumerable<TrainingProgramme>>(_configuration.FrameworksKey);
        }

        private TrainingProgramme Map(StandardSummary standard)
        {
            return new TrainingProgramme
            {
                Id = standard.Id,
                Title = standard.Title,
                Level = standard.Level,
                Duration = standard.Duration,
                MaxFunding = standard.MaxFunding
            };
        }

        private TrainingProgramme Map(FrameworkSummary framework)
        {
            return new TrainingProgramme
            {
                Id = framework.Id,
                Title = framework.Title,
                Level = framework.Level,
                Duration = framework.Duration,
                MaxFunding = framework.MaxFunding
            };
        }
    }
}
