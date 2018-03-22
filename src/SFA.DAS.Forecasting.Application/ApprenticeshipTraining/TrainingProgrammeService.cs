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
        private const string StandardsKey = "Standards";
        private const string FrameworksKey = "Frameworks";

        private readonly StartupConfiguration _configuration;
        private readonly ICache _cache;

        public TrainingProgrammeService(StartupConfiguration configuration, ICache cache)
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

        public async Task<IEnumerable<ITrainingProgramme>> GetStandards(bool refreshCache = false)
        {
            if (!await _cache.Exists(StandardsKey) || refreshCache)
            {
                var api = new StandardApiClient(_configuration.ApprenticeshipApiBaseUrl);

                var standards = api.FindAll().OrderBy(x => x.Title).ToList();

                await _cache.Set(StandardsKey, standards.Select(Map));
            }

            return await _cache.Get<IEnumerable<TrainingProgramme>>(StandardsKey);
        }

        public async Task<IEnumerable<ITrainingProgramme>> GetFrameworks(bool refreshCache = false)
        {
            if (!await _cache.Exists(FrameworksKey) || refreshCache)
            {
                var api = new FrameworkApiClient(_configuration.ApprenticeshipApiBaseUrl);

                var frameworks = api.FindAll().OrderBy(x => x.Title).ToList();

                await _cache.Set(FrameworksKey, frameworks.Select(Map));
            }

            return await _cache.Get<IEnumerable<TrainingProgramme>>(FrameworksKey);
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
