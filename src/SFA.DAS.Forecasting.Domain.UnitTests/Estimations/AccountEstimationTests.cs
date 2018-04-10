using System;
using System.Collections.Generic;
using System.Linq;
using AutoMoq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Commitments;
using SFA.DAS.Forecasting.Domain.Commitments.Validation;
using SFA.DAS.Forecasting.Domain.Events;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Estimations
{
    public class AccountEstimationTests
    {
        private AutoMoqer _moqer;

        private Account _account;
        private Commitment _commitment;
        private List<Commitment> _commitments;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();
            _account = new Account(1, 12000, 300);
            _moqer.SetInstance(_account);
            _commitment = new Commitment
            {
                EmployerAccountId = 1,
                ApprenticeshipId = 2,
                LearnerId = 3,
                StartDate = DateTime.Today,
                PlannedEndDate = DateTime.Today.AddMonths(25),
                MonthlyInstallment = 2100,
                NumberOfInstallments = 24,
                CompletionAmount = 3000
            };
            _commitments = new List<Commitment> { _commitment };
            var employerCommitments = new EmployerCommitments(1, _commitments, _moqer.GetMock<IEventPublisher>().Object, new CommitmentValidator());
            _moqer.SetInstance(employerCommitments);
        }

        private AccountEstimationModel ResolveEstimator()
        {
            return _moqer.Resolve<AccountEstimationModel>();
        }

        [Test]
        public void The_First_Month_In_The_Projection_Will_Be_The_Earliest_Month_In_Which_A_Virtual_Payment_Is_Taken()
        {
            var estimator = ResolveEstimator();
            estimator.BuildEstimations();
            Assert.AreEqual(DateTime.Today.Month + 1, accountProjection.Projections.FirstOrDefault()?.Month);
        }
    }
}