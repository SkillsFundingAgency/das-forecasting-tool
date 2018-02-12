using Nancy;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub
{
    public class SimpleApiModule : NancyModule
    {
        public SimpleApiModule()
        {
            Get("/greet", x => {
                return "Hola señoras y señores!";
            });

            Get("/api/accounts/{accountId}/levy", x => {
                if (!string.IsNullOrEmpty(x.accountId))
                {
                    return JsonConvert.SerializeObject(TestData.GetLevy(x.accountId));
                }

                return "";
            });
        }
    }
}
