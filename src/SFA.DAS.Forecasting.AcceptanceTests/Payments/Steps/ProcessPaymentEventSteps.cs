using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.Forecasting.AcceptanceTests.Services;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using TechTalk.SpecFlow;

namespace SFA.DAS.Forecasting.AcceptanceTests.Payments.Steps
{
    [Binding]
    public class ProcessPaymentEventSteps : StepsBase
    {
        //[Scope(Feature = "ProcessPaymentEvent")]
        //[BeforeFeature(Order = 1)]
        //public static void StartPaymentFunction()
        //{
        //    StartFunction("SFA.DAS.Forecasting.Payments.Functions");
        //}
        private AzureTableService _azureTableService;

		[BeforeScenario]
	    public void BeforeScenario()
	    {
		    _azureTableService = new AzureTableService(Config.AzureStorageConnectionString, Config.EmployerPaymentsTable);
		    _azureTableService.EnsureExists();
		    _azureTableService.DeleteEntities(EmployerAccountId.ToString());
	    }

	    [AfterScenario]
	    public void AfterSecnario()
	    {
		    _azureTableService.DeleteEntities(EmployerAccountId.ToString());
		    Thread.Sleep(1000);
	    }

		[Given(@"payment events have been created")]
        public async Task GivenPaymentEventsHaveBeenCreated()
        {
			await PostData(ValidData());
		}

	    [Given(@"events with invalid data have been created")]
	    public async Task WhenThereIsMissingEventData()
	    {
		    await PostData(InvalidData());
	    }

		[Then(@"there are (.*) payment events stored")]
        public void ThenThereArePaymentEventsStored(int expectedRecordsoBeSaved)
		{
			var _records = Do(() => _azureTableService?.GetRecords<PaymentEvent>(EmployerAccountId.ToString()), expectedRecordsoBeSaved, TimeSpan.FromMilliseconds(1000), 5);
			Assert.AreEqual(expectedRecordsoBeSaved, _records.Count(), message: $"Only {expectedRecordsoBeSaved} record should validate and be saved to the database");
		}

	    [Then(@"all of the data stored is correct")]
	    public void ThenAllOfTheDataStoredIsCorrect()
	    {
		    var _records = _azureTableService?.GetRecords<PaymentEvent>(EmployerAccountId.ToString())?.ToList();

		    Assert.IsTrue(_records.SingleOrDefault(m => m.ApprenticeshipId == 1234) != null);
		    Assert.IsTrue(_records.SingleOrDefault(m => m.ApprenticeshipId == 1235) != null);
		    Assert.IsTrue(_records.SingleOrDefault(m => m.ApprenticeshipId == 1236) != null);
	    }

	    [Then(@"the event with invalid data is not stored")]
	    public void ThenTheEventIsNotStored()
	    {
		    var _records = _azureTableService?.GetRecords<PaymentEvent>(EmployerAccountId.ToString());

		    Assert.AreEqual(0, _records.Count(m => m.EmployerAccountId.ToString().EndsWith("2")));
	    }

		private IEnumerable<string> ValidData()
	    {
		    return
			    new List<PaymentEvent> {
						new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 102,
						    ApprenticeshipId = 1235,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 103,
						    ApprenticeshipId = 1236,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    }
				    }
				    .Select(JsonConvert.SerializeObject);
	    }

	    private IEnumerable<string> InvalidData()
	    {
		    return
			    new List<PaymentEvent> {
						new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = -1,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = -1,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = -1,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.MinValue,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.MinValue,
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.MinValue,
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = -1,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = -1,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = -1,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = -1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = 0,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = 13,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentEvent {
						    EmployerAccountId = EmployerAccountId.ToString(),
						    Amount = 101,
						    ApprenticeshipId = 1234,
						    Id = Guid.NewGuid().ToString(),
						    Ukprn = 12345,
						    EarningDetails = new EarningDetails
						    {
							    StartDate = DateTime.Today,
							    PlannedEndDate = DateTime.Now.AddMonths(5),
							    ActualEndDate = DateTime.Now.AddMonths(6),
							    TotalInstallments = 18,
							    MonthlyInstallment = 1000,
							    CompletionAmount = 3000,
							    CompletionStatus = 1
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = -1
						    }
					    },
					}
				    .Select(JsonConvert.SerializeObject);
	    }

		private async Task PostData(IEnumerable<string> events)
	    {
		    var client = new HttpClient();

		    var url = Path.Combine(Config.FunctionBaseUrl, "EmployerPaymentEventHttpFunction");
		    foreach (var item in events)
		    {
			    await client.PostAsync(url, new StringContent(item));
		    }

		    Thread.Sleep(2000);
	    }

	    private static IEnumerable<T> Do<T>(
		    Func<IEnumerable<T>> action,
		    int expectedCount,
		    TimeSpan retryInterval,
		    int maxAttemptCount = 3)
	    {
		    for (int attempted = 0; attempted < maxAttemptCount; attempted++)
		    {
			    var a = action();
			    if (a.Count() == expectedCount)
			    {
				    return a;
			    }
			    Thread.Sleep(retryInterval);
		    }
		    return new List<T>();
	    }
    }
}
