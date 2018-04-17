using SFA.DAS.Apprenticeships.Api.Client;

namespace SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services
{
    public interface IStandardsService
    {

    }

    public class StandardsService: IStandardsService
    {
        public void GetCourses()
        {
            var api = new StandardApiClient();
            var standards = api.GetAllAsync();
        }
    }
}