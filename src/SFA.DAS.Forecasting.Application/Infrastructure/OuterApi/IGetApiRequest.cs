using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Application.Infrastructure.OuterApi
{
    public interface IGetApiRequest 
    {
        [JsonIgnore]
        string GetUrl { get; }
    }

}