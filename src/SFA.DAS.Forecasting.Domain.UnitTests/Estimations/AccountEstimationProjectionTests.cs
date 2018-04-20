using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Domain.Estimations;
using SFA.DAS.Forecasting.Domain.Estimations.Validation.VirtualApprenticeships;
using SFA.DAS.Forecasting.Domain.Events;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Estimations
{
    public class AccountEstimationProjectionTests
    {
        private AutoMoq.AutoMoqer _moqer;
        private List<Commitment> _commitments;
        private Account _account;
        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            _commitments = new List<Commitment>
            {
                new Commitment
                {
                    CompletionAmount = 100,
                    EmployerAccountId = 12345,
                    MonthlyInstallment = 50,
                    PlannedEndDate = new DateTime(2018, 5, 1),
                    StartDate = new DateTime(2018, 1, 1),
                    NumberOfInstallments = 5
                },
                new Commitment
                {
                    CompletionAmount = 100,
                    EmployerAccountId = 12345,
                    MonthlyInstallment = 50,
                    PlannedEndDate = new DateTime(2019, 7, 1),
                    StartDate = new DateTime(2019, 3, 1),
                    NumberOfInstallments = 5
                }
            };
            var employerCommitments = new EmployerCommitments(12345, _commitments, _moqer.GetMock<IEventPublisher>().Object, _moqer.GetMock<ICommitmentValidator>().Object);
            _moqer.SetInstance(employerCommitments);
            _account = new Account(12345, 10000, 0, 15000, 10000);
            _moqer.SetInstance(_account);
        }

        [Test]
        public void First_Month_Should_Be_Earliest_Payment_Date()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();
            estimationProjection.BuildProjections();
            var projection = estimationProjection.Projections.FirstOrDefault();
            Assert.IsNotNull(projection);
            Assert.IsTrue(projection.Month == 2 && projection.Year == 2018);
        }

        [Test]
        public void Last_Month_Should_Be_Last_Commitment_Date()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();
            estimationProjection.BuildProjections();
            var projection = estimationProjection.Projections.LastOrDefault();
            Assert.IsNotNull(projection);
            Assert.AreEqual(8, projection.Month, $"Expected to end in month 8 but last month was {projection.Month}");
        }

        [Test]
        public void Transfer_Balance_Should_Be_Reset_To_Transfer_Allowance_Each_May()
        {
            var estimationProjection = _moqer.Resolve<AccountEstimationProjection>();
            estimationProjection.BuildProjections();
            estimationProjection.Projections.Where(p => p.Month == 5).ToList()
                .ForEach(p => Assert.AreEqual((decimal)(_account.TransferAllowance), p.FutureFunds,$"Invalid transfer projection month. Year: {p.Year}, Expected balance: {_account.TransferAllowance - p.TotalCostOfTraining - p.CompletionPayments}, actual: {p.FutureFunds}"));
        }
    }
}