using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using Dapper;
using NUnit.Framework;
using SFA.DAS.Forecasting.Application.Converters;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Models.Payments;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;
using NamedCalendarPeriod = SFA.DAS.Forecasting.Application.Payments.Messages.NamedCalendarPeriod;

namespace SFA.DAS.Forecasting.AcceptanceTests.Payments.Steps
{
    [Binding]
    public class ProcessPaymentEventSteps : StepsBase
    {
        [Scope(Feature = "Process Payment Event")]
        [BeforeFeature(Order = 1)]
        public static void StartLevyFunction()
        {
            StartFunction("SFA.DAS.Forecasting.Payments.Functions");
            StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
        }

        [Given(@"I have no existing payments")]
        [Given(@"I have no existing payments recorded in the forecasting service")]
        public void GivenIHaveNoExistingPayments()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", Config.EmployerAccountId, DbType.Int64);
            Connection.Execute("Delete from Payment where employerAccountId = @employerAccountId or employerAccountId = '112233'",
                parameters, commandType: CommandType.Text);
        }

        [Given(@"I have no existing commitments")]
        [Given(@"I have no existing commitments recorded in the forecasting service")]
        public void GivenIHaveNoExistingCommitments()
        {
            DataContext.AccountProjectionCommitments.RemoveRange(DataContext.AccountProjectionCommitments.ToList());
            DataContext.Commitments.RemoveRange(DataContext.Commitments
                .Where(commitment => commitment.EmployerAccountId == Config.EmployerAccountId || commitment.EmployerAccountId == 112233).ToList());
            DataContext.SaveChanges();
        }

        [Given(@"I have made the following payments")]
        public void GivenIHaveMadeTheFollowingPayments(Table table)
        {
            Payments = table.CreateSet<TestPayment>().ToList();
            Assert.IsTrue(Payments.Any());
        }

        [Given(@"there is a corresponding commitment stored for each of the payments")]
        public void GivenThereIsACorrespondingCommitmentStoredForEachOfThePayments()
        {
            Payments.ForEach(payment =>
            {
                payment.ApprenticeshipId = payment.ApprenticeshipId == 0
                    ? new Random(Guid.NewGuid().GetHashCode()).Next(1, 999)
                    : payment.ApprenticeshipId;
                payment.LearnerId = payment.LearnerId == 0
                    ? new Random(Guid.NewGuid().GetHashCode()).Next(1, 999)
                    : payment.ApprenticeshipId;
                });

            var commitments = Payments.Select(payment => new TestCommitment
            {
                ActualEndDate = payment.ActualEndDate,
                ApprenticeName = payment.ApprenticeName,
                ApprenticeshipId = payment.ApprenticeshipId == 0 ? new Random(Guid.NewGuid().GetHashCode()).Next(1, 999) : payment.ApprenticeshipId,
                CompletionAmount = payment.CompletionAmount,
                CourseLevel = payment.CourseLevel,
                CourseName = payment.CourseName,
                EmployerAccountId = Config.EmployerAccountId,
                FundingSource = payment.FundingSource,
                InstallmentAmount = payment.InstallmentAmount,
                LearnerId = payment.LearnerId == 0 ? new Random(Guid.NewGuid().GetHashCode()).Next(1, 999) : payment.ApprenticeshipId,
                NumberOfInstallments = payment.NumberOfInstallments,
                ProviderName = payment.ProviderName,
                SendingEmployerAccountId = Config.EmployerAccountId,
                StartDate = payment.StartDate,
            }).ToList();

            InsertCommitments(commitments);
        }

        [Given(@"I made some invalid payments")]
        public void GivenIHaveMadeSomeInvalidPayments(Table table)
        {
            Payments = table.CreateSet<TestPayment>().ToList();
        }

        [When(@"the SFA Employer HMRC Payment service notifies the Forecasting service of the payment")]
        [When(@"the SFA Employer HMRC Payment service notifies the Forecasting service of the payments")]
        public void WhenTheSFAEmployerHMRCPaymentServiceNotifiesTheForecastingServiceOfThePayment()
        {
            for (var idx = 0; idx < Payments.Count; idx++)
            {
                var id = idx + 1;
                var payment = Payments[idx];
                payment.PaymentId = id.ToString();
                payment.ApprenticeshipId = payment.ApprenticeshipId > 0 ? payment.ApprenticeshipId : id;
                payment.ProviderId = id;
            }
            Payments.Select((payment, idx) => new PaymentCreatedMessage
            {
                Id = payment.PaymentId,
                EmployerAccountId = Config.EmployerAccountId,
                SendingEmployerAccountId = payment.SendingEmployerAccountId == 0 ? Config.EmployerAccountId : payment.SendingEmployerAccountId,
                Amount = payment.PaymentAmount,
                CollectionPeriod = new NamedCalendarPeriod
                {
                    Id = "1718-R01",
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year
                },
                DeliveryPeriod = new Application.Payments.Messages.CalendarPeriod
                {
                    Month = DateTime.Now.Month,
                    Year = DateTime.Now.Year
                },
                EarningDetails = new EarningDetails
                {
                    ActualEndDate = payment.ActualEndDateValue ?? DateTime.MinValue,
                    StartDate = payment.StartDateValue,
                    PlannedEndDate = payment.PlannedEndDate,
                    TotalInstallments = payment.NumberOfInstallments,
                    MonthlyInstallment = payment.InstallmentAmount,
                    CompletionAmount = payment.CompletionAmount,
                    EndpointAssessorId = "654321",
                    CompletionStatus = 1,
                    RequiredPaymentId = Guid.NewGuid()
                },
                ApprenticeshipId = payment.ApprenticeshipId,
                ProviderName = payment.ProviderName,
                Ukprn = payment.ProviderId,
                ApprenticeName = payment.ApprenticeName,
                CourseLevel = payment.CourseLevel,
                CourseName = payment.CourseName,
                Uln = idx,
                CourseStartDate = payment.StartDateValue,
                FundingSource = payment.SendingEmployerAccountId == 0 || payment.SendingEmployerAccountId == EmployerAccountId ? FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Levy) : FundingSourceConverter.ConvertToApiFundingSource(FundingSource.Transfer)
            })
            .ToList()
            .ForEach(paymentEvent =>
            {
                var payload = paymentEvent.ToJson();
                var url = Config.PaymentFunctionUrl;
                Console.WriteLine($"Sending payment event to payment function: {url}, Payload: {payload}");
                var response = HttpClient.PostAsync(url, new StringContent(payload, Encoding.UTF8, "application/json")).Result;
                Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            });
        }

        [Then(@"the Forecasting Payment service should store the payment declarations")]
        public void ThenTheForecastingPaymentServiceShouldStoreThePaymentDeclarations()
        {
            WaitForIt(() =>
            {
                var payments = DataContext.Payments.Where(m => m.EmployerAccountId == Config.EmployerAccountId).ToList();

                foreach (var payment in Payments)
                {
                    if (payments.Any(p => payment.PaymentId == p.ExternalPaymentId)) continue;
                    var msg = $"Payment not found. Payment: {payment.ToJson()}";
                    return Tuple.Create(false, msg);
                }
                return Tuple.Create(true, string.Empty);

            }, "Failed to find all the payments.");
        }

        [Then(@"the Forecasting Payment service should store the payment declarations receiving employer (.*) from sending employer (.*)")]
        public void ThenTheForecastingPaymentServiceShouldStoreThePaymentDeclarationsReceivingEmployerFromSendingEmployer(int receivningEmployerId, int sendingEmployerId)
        {
            WaitForIt(() =>
            {
                var payments = DataContext.Payments
                    .Where(m => m.EmployerAccountId == receivningEmployerId
                             && m.SendingEmployerAccountId == sendingEmployerId
                             && m.FundingSource == FundingSource.Transfer)
                    .ToList();

                var paymentsSaved = Payments.Count(p => payments.Any(expected => expected.ExternalPaymentId == p.PaymentId));
                var msg = $"{paymentsSaved} of expected {Payments.Count()} Payment found.";

                var pass = Payments.All(p => payments.Any(expected => expected.ExternalPaymentId == p.PaymentId));
                return Tuple.Create(pass, msg);

            }, $"Failed to find all the payments.");
        }


        [Then(@"the Forecasting Payment service should store the commitment declarations")]
        public void ThenTheForecastingPaymentServiceShouldStoreTheCommitmentDeclarations()
        {
            WaitForIt(() =>
            {
                foreach (var payment in Payments)
                {
                    var commitmentsCount = DataContext.Commitments
                        .Count(m => m.EmployerAccountId == Config.EmployerAccountId
                                 && m.ApprenticeshipId == payment.ApprenticeshipId
                                 && m.ProviderId == payment.ProviderId);

                    return Tuple.Create(commitmentsCount == 1, $"{payment.ToJson()}");
                }
                return Tuple.Create(false, $"");
            }, "Failed to find all the commitments.");
        }

        [Then(@"the Forecasting Payment service should store the commitment declarations for receiving employer (.*) from sending employer (.*)")]
        public void ThenTheForecastingPaymentServiceShouldStoreTheCommitmentDeclarationsForReceivingEmployerFromSendingEmployer(int receivingEmployerId, int sendingEmployerId)
        {
            WaitForIt(() =>
            {
                foreach (var payment in Payments)
                {
                    var commitmentsCount = DataContext.Commitments
                        .Count(m => m.EmployerAccountId == receivingEmployerId
                                 && m.SendingEmployerAccountId == sendingEmployerId
                                 && m.ApprenticeshipId == payment.ApprenticeshipId
                                 && m.FundingSource == FundingSource.Transfer
                                 && m.ProviderId == payment.ProviderId);
                                 
                    return Tuple.Create(commitmentsCount == 1, $"{payment.ToJson()}");
                }
                return Tuple.Create(false, $"");
            }, "Failed to find all the commitments.");
        }


        [Then(@"the Forecasting Payment service should not store the payments")]
        public void ThenTheForecastingPaymentServiceShouldNotStoreThePaymentDeclarations()
        {
            Thread.Sleep(Config.TimeToWait);

            var count = DataContext.Payments
             .Count(m => m.EmployerAccountId == Config.EmployerAccountId
                    && m.CollectionPeriod.Year == DateTime.Now.Year
                    && m.CollectionPeriod.Month == DateTime.Now.Month);

            var msg = $"Looking for Payments.Employer Account Id: { Config.EmployerAccountId}, Collection Period Year: { DateTime.Now.Year}, Collection Period Month: { DateTime.Now.Month}";

            Assert.AreEqual(0, count, message: msg);
        }

        [Then(@"the Forecasting Payment service should not store commitments")]
        public void ThenTheForecastingPaymentServiceShouldNotStoreTheCommitments()
        {
            Thread.Sleep(Config.TimeToWait);
            var count = DataContext.Commitments
                .Count(m => m.EmployerAccountId == Config.EmployerAccountId );
                var msg = $"Looking for Commitments. Employer Account Id: {Config.EmployerAccountId}, Collection Period Year: {DateTime.Now.Year}, Collection Period Month: {DateTime.Now.Month}";

            Assert.AreEqual(0, count, msg);
        }

        [Then(@"there will be (.*) commitment for employer (.*)")]
        public void ThenThereWillBeCommitmentForEmployer(int expectedCount, int employerId)
        {
            WaitForIt(() =>
            {
                var count = DataContext.Commitments
                    .Where(m => m.EmployerAccountId == employerId)
                    .Count();

                var msg = $"Looking for Commitments. Found {count} of {expectedCount}. Employer Account Id: {employerId}, Collection Period Year: {DateTime.Now.Year}, Collection Period Month: {DateTime.Now.Month}";

                return Tuple.Create(count == expectedCount, msg);

            }, $"Failed to find all commitments. Expected: {expectedCount}");
        }

        [Then(@"apprenticeship with id (.*) should have an end date")]
        public void ThenApprenticeshipWithIdShouldHaveAnEndDate(int apprenticeshipId)
        {
            WaitForIt(() =>
            {
                var commitment = DataContext.Commitments
                .AsNoTracking()
                    .Single(m => m.ApprenticeshipId == apprenticeshipId);

                var msg = $"Looking for Commitment {apprenticeshipId}. ActualEndDate: {commitment.ActualEndDate}.";

                return Tuple.Create(commitment.ActualEndDate != null && commitment.ActualEndDate > DateTime.MinValue, msg);

            }, $"Failed finding commitment ({apprenticeshipId}) with actual end date");
        }

        [Then(@"apprenticeship with id (.*) should not have an actual end date")]
        public void ThenApprenticeshipWithIdShouldNotHaveAnActualEndDate(int apprenticeshipId)
        {
            WaitForIt(() =>
            {
                var commitment = DataContext.Commitments
                    .AsNoTracking()
                    .Single(m => m.ApprenticeshipId == apprenticeshipId);

                var msg = $"Looking for Commitment {apprenticeshipId}. ";

                return Tuple.Create(commitment.ActualEndDate == null || commitment.ActualEndDate == DateTime.MinValue, msg);

            }, $"Failed finding commitment ({apprenticeshipId}) with actual end date");
        }

        [Then(@"apprenticeship with id (.*) should have completion amount of (.*) and monthly installment of (.*)")]
        public void ThenApprenticeshipWithIdShouldHaveCompletionAmountOfAndMonthlyInstallnebtOf(int apprenticeshipId, int completionAmount, Decimal installmentAmount)
        {
            WaitForIt(() =>
            {
                var commitment = DataContext.Commitments
                    .AsNoTracking()
                    .Single(m => m.ApprenticeshipId == apprenticeshipId);

                var msg = $"Looking for Commitment {apprenticeshipId}. CompletionAmount: {commitment.CompletionAmount} and InstallmentAmount: {commitment.MonthlyInstallment}";

                return Tuple.Create(commitment.CompletionAmount == completionAmount && commitment.MonthlyInstallment == installmentAmount, msg);

            }, $"Failed finding correct values for commitment ({apprenticeshipId}). Expecting CompletionAmount of {completionAmount} and InstallmentAmount of {installmentAmount}");
        }

        [Then(@"the Forecasting Payment service should record that the commitment has ended")]
        public void ThenTheForecastingPaymentServiceShouldStoreTheStoppedCommitment()
        {
            WaitForIt(() =>
                {
                    return Payments.All(payment => DataContext.Commitments.Any(c =>
                        c.EmployerAccountId == Config.EmployerAccountId &&
                        c.ApprenticeshipId == payment.ApprenticeshipId && c.ActualEndDate != null));
                },"Failed to find the ended commitment");
        }
    }
}
