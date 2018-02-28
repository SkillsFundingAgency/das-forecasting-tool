using Dapper;
using FluentAssertions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
        private static long _employerAccountId = 8509;
        private long _substitutionEmployerAccountId = 112233;

        [BeforeFeature(Order = 1)]
        public static void StartPreLoadLevyEvent()
        {
          //  _apiHost = new ApiHost();
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
            var item = "{\"EmployerAccountIds\":[8509],\"PeriodYear\":\"2017\",\"PeriodMonth\":1,\"PeriodId\": \"1617-R10\"}";

            var client = new HttpClient();
            await client.PostAsync(Config.PaymentPreLoadHttpFunction, new StringContent(item));
        }

        [Given(@"I trigger PreLoadPayment function some employers with a substitution id")]
        public async Task GivenITriggerPreLoadPaymentFunctionSomeEmployersWithASubstitutionId()
        {
            var item = "{\"EmployerAccountIds\":[" + _employerAccountId + "],\"PeriodYear\":\"2017\",\"PeriodMonth\":1,\"PeriodId\": \"1617-R10\", \"SID\": " + _substitutionEmployerAccountId + "}";

            var client = new HttpClient();
            await client.PostAsync(Config.PaymentPreLoadHttpFunction, new StringContent(item));
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
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", _employerAccountId, DbType.Int64);
                var count = Connection.ExecuteScalar<int>("Select Count(*) from Payment where EmployerAccountId = @employerAccountId"
                        , param: parameters, commandType: CommandType.Text);
                return count == 1;
            }, "Failed to find all the payments.");

            //_apiHost.Dispose();
        }

        [Then(@"there will be payments for the employer and no sensitive data will have been stored in the database")]
        public void ThenThereWillBePaymentsForAllTheEmployersWithNoSensitiveData()
        {
            // ToDo: this
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", _substitutionEmployerAccountId, DbType.Int64);
            var payments = Connection.ExecuteScalar<IEnumerable<Models.Payments.Payment>>("Select * from Payment where EmployerAccountId = @employerAccountId"
                    , param: parameters, commandType: CommandType.Text);

            var payment = payments.FirstOrDefault();

            payment.Should().NotBeNull();

            payment.ApprenticeshipId.Should().NotBe(1); // Not be the stame as input!
            payment.ExternalPaymentId.Should().NotBe(""); // Not be the stame as input!
            payment.EmployerAccountId.Should().Be(_substitutionEmployerAccountId);
            payment.ProviderId.Should().Be(1);

            payment.Amount.Should().Be(200); // Same as input
        }

        [Then(@"there will be commitment for the employer and no sensitive data will have been stored in the database")]
        public void ThenThereWillBeCommitmentForTheEmployersWithNoSensitiveData()
        {
            // ToDo: this
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", _substitutionEmployerAccountId, DbType.Int64);
            var payments = Connection.ExecuteScalar<IEnumerable<Models.Commitments.Commitment>>("Select * from Commitment where EmployerAccountId = @employerAccountId"
                    , param: parameters, commandType: CommandType.Text);

            var payment = payments.FirstOrDefault();

            payment.Should().NotBeNull();

            payment.ApprenticeshipId.Should().NotBe(1); // Not be the stame as input!
            payment.EmployerAccountId.Should().Be(_substitutionEmployerAccountId);
            payment.ProviderId.Should().Be(1);
            payment.ProviderName.Should().Be("Provider Name");

            payment.ApprenticeName.Should().Be("Apprentice Name");
            payment.CourseName.Should().Be(""); // Same as input
            payment.CourseLevel.Should().Be(1); // Same as input
        }

        private void ClearDatabase()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@id1", _employerAccountId, DbType.Int64);
            parameters.Add("@id2", _substitutionEmployerAccountId, DbType.Int64);
            var count = Connection.ExecuteScalar<int>("DELETE Payment WHERE EmployerAccountId IN [@id1, @id2]"
                    , param: parameters, commandType: CommandType.Text);
        }
    }
}
