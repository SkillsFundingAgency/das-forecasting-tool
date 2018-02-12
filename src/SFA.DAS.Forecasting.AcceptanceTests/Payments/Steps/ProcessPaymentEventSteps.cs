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
using SFA.DAS.Forecasting.Models.Payments;
using TechTalk.SpecFlow;
using CollectionPeriod = SFA.DAS.Forecasting.Application.Payments.Messages.CollectionPeriod;

namespace SFA.DAS.Forecasting.AcceptanceTests.Payments.Steps
{
    [Binding]
    public class ProcessPaymentEventSteps : StepsBase
    {
		[Scope(Feature = "ProcessPaymentEvent")]
		[BeforeFeature(Order = 1)]
		public static void StartPaymentFunction()
		{
			StartFunction("SFA.DAS.Forecasting.Payments.Functions");
		}

		[Given(@"payment events have been created")]
        public void GivenPaymentEventsHaveBeenCreated()
        {
			ScenarioContext.Current.Pending();
		}

	    [Given(@"events with invalid data have been created")]
	    public void WhenThereIsMissingEventData()
	    {
	        ScenarioContext.Current.Pending();
	    }

		[Then(@"there are (.*) payment events stored")]
        public void ThenThereArePaymentEventsStored(int expectedRecordsoBeSaved)
		{
            ScenarioContext.Current.Pending();
		}

		[Then(@"all of the data stored is correct")]
		public void ThenAllOfTheDataStoredIsCorrect()
		{
		    ScenarioContext.Current.Pending();
		}

        [Then(@"the aggregation for the total cost of training has been created properly")]
		public void ThenTotalCostAggregationIsCreated()
		{
		    ScenarioContext.Current.Pending();
		}

        [Then(@"the event with invalid data is not stored")]
	    public void ThenTheEventIsNotStored()
	    {
	        ScenarioContext.Current.Pending();
	    }

        private IEnumerable<string> ValidData()
	    {
		    return
			    new List<PaymentCreatedMessage> {
						new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
								EndpointAssessorId = Guid.NewGuid().ToString()
						    },
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
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
			    new List<PaymentCreatedMessage> {
						new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = -1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = 0,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = 13,
							    Year = DateTime.Now.Year
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
							    CompletionStatus = 1,
							    EndpointAssessorId = Guid.NewGuid().ToString()
							},
						    CollectionPeriod = new CollectionPeriod
						    {
							    Id = "123",
							    Month = DateTime.Now.Month,
							    Year = -1
						    }
					    },
					    new PaymentCreatedMessage {
						    EmployerAccountId = EmployerAccountId,
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
    }
}
