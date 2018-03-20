using Dapper;
using NUnit.Framework;
using SFA.DAS.Forecasting.Web.Automation;
using System;
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

        [Given(@"I have generated the following completion payments")]
        public void GivenIHaveGeneratedTheFollowingCompletionPayments(Table table)
        {
            var pendingComletions = table.CreateSet<TestPendingCompletion>().ToList();

            DeletePendingCompletions();
            Store(pendingComletions);
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
            Connection.Execute("Delete from AccountPendingCompletionPayment where employerAccountId = @employerAccountId", parameters, commandType: CommandType.Text);
        }

        public void Store(IEnumerable<TestPendingCompletion> pendingCompletions)
        {
            var employerAccountId = long.Parse(Config.EmployerAccountID);
            using (var txScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                var sql = @"Insert Into [dbo].[AccountPendingCompletionPayment] Values 
                           (@employerAccountId,
                           @amount,
                           @createdOn)";

                foreach (var pendingCompletion in pendingCompletions)
                {
                    var createdOn = DateTime.Parse(pendingCompletion.CreatedOn);

                    var parameters = new DynamicParameters();
                    parameters.Add("@employerAccountId", employerAccountId, DbType.Int64);
                    parameters.Add("@amount", pendingCompletion.Amount, DbType.Decimal);
                    parameters.Add("@createdOn", pendingCompletion.CreatedOn, DbType.DateTime);

                    Connection.Execute(sql, parameters, commandType: CommandType.Text);
                }
                txScope.Complete();
            }
        }

        public class TestPendingCompletion
        {
            public decimal Amount { get; set; }
            public string CreatedOn { get; set; }
        }
    }
}
