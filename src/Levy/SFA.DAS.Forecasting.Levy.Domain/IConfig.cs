
namespace SFA.DAS.Forecasting.Levy.Domain
{
    public interface IConfig
    {
        string Hello { get; set; }
    }

    public class Config : IConfig
    {
        public string Hello { get; set; }
    }
}
