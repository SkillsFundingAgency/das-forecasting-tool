using Dapper;
using SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Payments.Steps
{
    [Scope(Feature = FeatureName)]
    [Binding]
    public class PreLoadPaymentsSteps : StepsBase
    {
        private const string FeatureName = "PreLoadPayments";
        private static IEnumerable<long> _accountIds = new List<long> { 8509 };
        private static string Url = Path.Combine(Config.PaymentFunctionUrl, "PaymentPreLoadHttpFunction");
        private static ApiHost _apiHost;

        [BeforeFeature(Order = 1)]
        public static void StartPreLoadLevyEvent()
        {
            _apiHost = new ApiHost();
            StartFunction("SFA.DAS.Forecasting.Payments.Functions");
            Thread.Sleep(1000);
        }

        [BeforeScenario]
        public void BeforeScenario()
        {
            ClearDatabase();
            Thread.Sleep(500);
        }

        [AfterScenario(Order = 1)]
        public void AfterScenario()
        {
            ClearDatabase();
        }

        [Given(@"I trigger PreLoadPayment function some employers")]
        public async Task GivenITriggerPreLoadPaymentFunctionSomeEmployers()
        {
            var item = "{\"EmployerAccountIds\":[\"MN4YKL\",\"MGXLRV\",\"MPN4YM\"],\"PeriodYear\":\"2017\",\"PeriodMonth\":1,\"PeriodId\": \"1617-R10\"}";

            var client = new HttpClient();
            await client.PostAsync(Url, new StringContent(item));
        }
        
        [When(@"data have been processed")]
        public void WhenDataHaveBeenProcessed()
        {
            Thread.Sleep(500);
        }
        
        [Then(@"there will be payments for all the employers")]
        public void ThenThereWillBePaymentsForAllTheEmployers()
        {
            WaitForIt(() =>
            {
                foreach (var accountId in _accountIds)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", accountId, DbType.Int64);
                    var count = Connection.ExecuteScalar<int>("Select Count(*) from Payment where EmployerAccountId = @employerAccountId"
                         , param: parameters, commandType: CommandType.Text);
                    return count == 1;
                }
                return false;
            }, "Failed to find all the payments.");

            _apiHost.Dispose();
        }

        private void ClearDatabase()
        {
            foreach (var accountId in _accountIds)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", accountId, DbType.Int64);
                var count = Connection.ExecuteScalar<int>("DELETE Payment WHERE EmployerAccountId = @employerAccountId"
                     , param: parameters, commandType: CommandType.Text);

            }
        }
    }
}
