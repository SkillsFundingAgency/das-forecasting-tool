using Dapper;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.AcceptanceTests.Infrastructure;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Provider.Events.Api.Types;
using System;
using System.Collections.Generic;
using System.Data;
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
        private TableStorageService _tableStorageService;
        private readonly Config _settings;
        private static long _accountId = 497;

        public PreLoadPaymentsSteps()
        {
            _tableStorageService = ParentContainer.GetInstance<TableStorageService>();
            _settings = ParentContainer.GetInstance<Config>();
        }

        [BeforeFeature(Order = 1)]
        public static void StartPreLoadLevyEvent()
        {
            StartFunction("SFA.DAS.Forecasting.Payments.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
            StartFunction("SFA.DAS.Forecasting.PreLoad.Functions");

            Thread.Sleep(1000);
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            ClearDatabase();
            await SetUpEmployerData();

            var client = new HttpClient();
            var payment = JsonConvert.SerializeObject(GetPayment());
            await client.PostAsync(Config.ApiInsertPaymentUrl, new StringContent(payment));

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
            var item = "{\"EmployerAccountIds\":[\"MJK9XV\", \"MN4YKL\",\"MGXLRV\",\"MPN4YM\"],\"PeriodYear\":\"2017\",\"PeriodMonth\":5,\"PeriodId\": \"1617-R10\"}";

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
            var count = 0;
            WaitForIt(() =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", _accountId, DbType.Int64);
                count = Connection.ExecuteScalar<int>("Select Count(*) from Payment where EmployerAccountId = @employerAccountId"
                        , param: parameters, commandType: CommandType.Text);
                    return count == 1;

            }, $"Failed to find all the payments. Found: {count}");
        }

        private void ClearDatabase()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", _accountId, DbType.Int64);
            var count = Connection.ExecuteScalar<int>("DELETE Payment WHERE EmployerAccountId = @employerAccountId"
                    , param: parameters, commandType: CommandType.Text);
        }

        private async Task SetUpEmployerData()
        {
            var year = 2017;
            var month = 5;
            var p2 = new EmployerPayment
            {
                    Ukprn = 10001378,
                    Uln = 2002,
                    AccountId = _accountId,
                    ApprenticeshipId = 6666,
                    CollectionPeriodId = "1617-r10",
                    CollectionPeriodMonth = month,
                    CollectionPeriodYear = year,
                    Amount = 50.00000m,
                    PaymentMetaDataId = 690,
                    ProviderName = "CHESTERFIELD COLLEGE",
                    StandardCode = 4,
                    FrameworkCode = 0,
                    ProgrammeType = 0,
                    PathwayCode = 0,
                    PathwayName = null,
                    ApprenticeshipCourseName = "Chemical Engineering",
                    ApprenticeshipCourseStartDate = DateTime.Parse("2017-01-09"),
                    ApprenticeshipCourseLevel = 2,
                    ApprenticeName = "John Doe",
                    FundingSource = Models.Payments.FundingSource.Levy,
                    PaymentId = Guid.Parse("f97840b3-d3bf-429c-bc3c-8a877f4f26f8") // Need to match Payment from ProviderEvents API // IN: ProviderEventTestData.cs
            };
            
            _tableStorageService.SetTable(_settings.StubEmployerPaymentTable);
            await _tableStorageService.Store(new List<EmployerPayment> { p2 }, _accountId.ToString(), $"{year}-{month}");
        }

        internal static PageOfResults<Provider.Events.Api.Types.Payment> GetPayment()
        {
            var por = new PageOfResults<Provider.Events.Api.Types.Payment>();
            por.PageNumber = 1;
            por.TotalNumberOfPages = 1;
            por.Items = new List<Provider.Events.Api.Types.Payment>
            {
                new Provider.Events.Api.Types.Payment
                {
                    Id = "f97840b3-d3bf-429c-bc3c-8a877f4f26f8",
                    ApprenticeshipId = 11002,

                    CollectionPeriod = new NamedCalendarPeriod
                    {
                        Id = "1617-r10",
                        Year = 2017,
                        Month = 5
                    },
                    EarningDetails = new List<Earning>
                    {
                        new Earning
                        {
                            ActualEndDate = DateTime.Parse("2017-03-01"),
                            CompletionAmount = 5000,
                            RequiredPaymentId = Guid.Parse("f97840b3-d3bf-429c-bc3c-8a877f4f26f8"),
                            CompletionStatus = 1,
                            MonthlyInstallment = 300,
                            TotalInstallments = 24,
                            StartDate = DateTime.Parse("2018-01-01"),
                            PlannedEndDate = DateTime.Parse("2020-01-01"),
                            EndpointAssessorId = "EOId-1"
                        }
                    },
                    ContractType = ContractType.ContractWithSfa,
                    FundingSource = Provider.Events.Api.Types.FundingSource.Levy,
                    TransactionType = TransactionType.Balancing
                }
            }
            .ToArray();

            return por;
        }
    }
}
