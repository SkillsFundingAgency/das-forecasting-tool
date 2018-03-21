using Dapper;
using NUnit.Framework;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Web.Automation;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Transactions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.StepDefinition
{
    [Binding]
    public class FundingProjectionPage_PendingCompletionSteps : StepsBase
    {
        protected IDbConnection Connection => NestedContainer.GetInstance<IDbConnection>();

        [Given(@"I have generated the following commitments")]
        public void GivenIHaveGeneratedTheFollowingCommitments(Table table)
        {
            var commitments = table.CreateSet<Commitment>().ToList();

            DeletePendingCompletions();
            Store(commitments);
        }


        [Then(@"Pending completion payments should be (.*)")]
        public void ThenPendingCompletionPaymentsShouldBe(string expectedPendingCompletionValue)
        {
            var page = Get<FundingProjectionPage>();
            var pendingCompletionValue = page.PendingComletionPayment.Text;
            Assert.AreEqual(expectedPendingCompletionValue, pendingCompletionValue);
        }


        protected void DeletePendingCompletions()
        {
            var parameters = new DynamicParameters();
            parameters.Add("@employerAccountId", long.Parse(Config.EmployerAccountID), DbType.Int64);
            Connection.Execute("Delete from Commitment where employerAccountId = @employerAccountId", parameters, commandType: CommandType.Text);
        }

        public void Store(IEnumerable<Commitment> commitments)
        {
            var employerAccountId = long.Parse(Config.EmployerAccountID);
            using (var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var sql = @"Insert Into Commitment 
                        Values (
                            @employerAccountId,
                            @learnerId,
                            @providerId,
                            @providerName,
                            @apprenticeshipId,
                            @apprenticeName,
                            @courseName,
                            @courseLevel,
                            @startDate,
                            @plannedEndDate,
                            @actualEndDate,
                            @completionAmount,
                            @monthlyInstallment,
                            @numberOfInstallments)";

                foreach (var commitment in commitments)
                {
                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", commitment.EmployerAccountId, DbType.Int64);
                    parameters.Add("@learnerId", commitment.LearnerId, DbType.Int64);
                    parameters.Add("@providerId", commitment.ProviderId, DbType.Int64);
                    parameters.Add("@providerName", commitment.ProviderName, DbType.String);
                    parameters.Add("@apprenticeshipId", commitment.ProviderId, DbType.Int64);
                    parameters.Add("@apprenticeName", commitment.ApprenticeName, DbType.String);
                    parameters.Add("@courseName", commitment.CourseName, DbType.String);
                    parameters.Add("@courseLevel", commitment.CourseLevel, DbType.Int16);
                    parameters.Add("@startDate", commitment.StartDate, DbType.DateTime);
                    parameters.Add("@plannedEndDate", commitment.PlannedEndDate, DbType.DateTime);
                    parameters.Add("@actualEndDate", null, DbType.DateTime);
                    parameters.Add("@completionAmount", commitment.CompletionAmount, DbType.Decimal);
                    parameters.Add("@monthlyInstallment", commitment.MonthlyInstallment, DbType.Int32);
                    parameters.Add("@numberOfInstallments", commitment.NumberOfInstallments, DbType.Int32);


                    Connection.Execute(sql, parameters, commandType: CommandType.Text);
                }
                txScope.Complete();
            }
        }
    }
}
