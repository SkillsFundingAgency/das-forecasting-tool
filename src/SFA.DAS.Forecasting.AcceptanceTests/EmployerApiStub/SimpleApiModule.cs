using System;
using Nancy;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub
{
    public class SimpleApiModule : NancyModule
    {
        public static decimal AccountBalance { get; set; }

        public SimpleApiModule()
        {
            Get("/greet", x => "Hola señoras y señores!");

            Get("/api/accounts/{accountId}/levy", x => {
                if (!string.IsNullOrEmpty(x.accountId))
                {
                    return JsonConvert.SerializeObject(TestData.GetLevy(x.accountId));
                }

                return "";
            });

            Get("/api/accounts/{accountId}", p =>
            {
                try
                {
                    return TestData.GetAccountDetail(AccountBalance);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
        }
    }
}
