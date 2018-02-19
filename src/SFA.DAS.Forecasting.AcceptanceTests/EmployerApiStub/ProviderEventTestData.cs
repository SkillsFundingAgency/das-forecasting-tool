using Newtonsoft.Json;
using SFA.DAS.Provider.Events.Api.Types;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.AcceptanceTests.EmployerApiStub
{
    public class ProviderEventTestData
    {
        internal static PageOfResults<Payment> GetPayment(string accountId, string period_id, string page_nubmber)
        {
            var por = new PageOfResults<Payment>();
            por.PageNumber = 1;
            por.TotalNumberOfPages = 1;
            por.Items = new List<Payment>
            {
                new Payment
                {
                    Id = "1B796A6A-C96F-4C72-94AB-02A302358D4B",
                    ApprenticeshipId = 11002,

                    CollectionPeriod = new NamedCalendarPeriod
                    {
                        Id = "16-17R5",
                        Year = 2017,
                        Month = 1
                    },
                    EarningDetails = new List<Earning>
                    {
                        new Earning
                        {
                            ActualEndDate = DateTime.Parse("2017-03-01"),
                            CompletionAmount = 5000,
                            RequiredPaymentId = Guid.Parse("1B796A6A-C96F-4C72-94AB-02A302358D4B"),
                            CompletionStatus = 1,
                            MonthlyInstallment = 300,
                            TotalInstallments = 24,
                            StartDate = DateTime.Parse("2018-01-01"),
                            PlannedEndDate = DateTime.Parse("2020-01-01"),
                            EndpointAssessorId = "EOId-1"
                        }
                    },
                    ContractType = ContractType.ContractWithSfa,
                    FundingSource = FundingSource.Levy,
                    TransactionType = TransactionType.Balancing
                }
            }
            .ToArray();

            return por;
            //return JsonConvert.SerializeObject(por);
        }

        internal static string GetPayment2(string accountId, string period_id, string page_nubmber)
        {
            return Eid8509;
        }

        private static string Eid8509 => @"{
                                          ""PageNumber"": 1,
                                          ""TotalNumberOfPages"": 1,
                                          ""Items"": [
                                            {
                                              ""Id"": ""631E716C-C760-48BC-9BA1-9EAF78F6BF6C"",
                                              ""Ukprn"": 234432,
                                              ""Uln"": 45126,
                                              ""DeliveryPeriod"": {
                                                ""Month"": 4,
                                                ""Year"": 2017
                                              },
                                              ""CollectionPeriod"": {
                                                ""Id"": ""1617-R09"",
                                                ""Month"": 4,
                                                ""Year"": 2017
                                              },
                                              ""EvidenceSubmittedOn"": ""2017-01-09T10:40:11.68"",
                                              ""FundingSource"": ""CoInvestedEmployer"",
                                              ""TransactionType"": ""Learning"",
                                              ""Amount"": 110.76923,
                                              ""FrameworkCode"": 550,
                                              ""ProgrammeType"": 20,
                                              ""PathwayCode"": 6,
                                              ""ContractType"": ""ContractWithSfa"",
                                              ""EarningDetails"": [
                                                {
                                                  ""RequiredPaymentId"": ""57999b0e-12ef-44b8-ace4-267baf68c5aa"",
                                                  ""StartDate"": ""2017-08-01T00:00:00"",
                                                  ""PlannedEndDate"": ""2018-08-01T00:00:00"",
                                                  ""ActualEndDate"": ""0001-01-01T00:00:00"",
                                                  ""CompletionStatus"": 1,
                                                  ""CompletionAmount"": 240,
                                                  ""MonthlyInstallment"": 80,
                                                  ""TotalInstallments"": 12
                                                }
                                              ]
                                            }
                                          ]
                                        }";
    }
}
