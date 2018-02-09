using SFA.DAS.EAS.Account.Api.Client;

namespace SFA.DAS.Forecasting.Functions.Framework.Infrastructure
{
    public interface IConfig
    {
        string AllowedHashstringCharacters { get; set; }
        string Hashstring { get; set; }

        AccountApiConfiguration AccountApi { get; set; }
    }
    public class Config : IConfig
    {
        public string AllowedHashstringCharacters { get; set; }

        public string Hashstring { get; set; }

        public AccountApiConfiguration AccountApi { get; set; }
    } 
}