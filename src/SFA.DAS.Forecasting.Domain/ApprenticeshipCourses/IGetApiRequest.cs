using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Domain.ApprenticeshipCourses
{
    public interface IGetApiRequest :IBaseApiRequest
    {
        [JsonIgnore]
        string GetUrl { get; }
    }

    public interface IBaseApiRequest
    {
        [JsonIgnore]
        string BaseUrl { get; }
    }
}