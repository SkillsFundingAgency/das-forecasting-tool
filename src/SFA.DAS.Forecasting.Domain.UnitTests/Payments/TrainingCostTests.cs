using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Forecasting.Domain.Payments.Aggregates;
using SFA.DAS.Forecasting.Domain.Payments.Entities;
using SFA.DAS.Forecasting.Domain.Payments.Repositories;

namespace SFA.DAS.Forecasting.Domain.UnitTests.Payments
{
	[TestFixture]
	public class TrainingCostTests
	{
		private Mock<ITrainingCostRepository> _trainingCostRepository;
		private TrainingCost _service;

		[SetUp]
		public void SetUp()
		{
			_trainingCostRepository = new Mock<ITrainingCostRepository>();

			_service = new TrainingCost(_trainingCostRepository.Object);
		}

		[Test]
		public void Aggregates_Employer_Payments()
		{
			var payments = new List<Payment>
			{
				new Payment
				{
					EmployerAccountId = "1234",
					CollectionPeriod = new CollectionPeriod
					{
						Month = 1,
						Year = 2018
					},
					Amount = 12
				},
				new Payment
				{
					EmployerAccountId = "1234",
					CollectionPeriod = new CollectionPeriod
					{
						Month = 1,
						Year = 2018
					},
					Amount = 21
				},
				new Payment
				{
					EmployerAccountId = "1234",
					CollectionPeriod = new CollectionPeriod
					{
						Month = 1,
						Year = 2018
					},
					Amount = 42
				},
				new Payment
				{
					EmployerAccountId = "1234",
					CollectionPeriod = new CollectionPeriod
					{
						Month = 1,
						Year = 2018
					},
					Amount = 24
				},
			};

			var actual = _service.AggregateEmployerPayments(payments);

			Assert.AreEqual(actual.EmployerAccountId, "1234");
			Assert.AreEqual(actual.PeriodMonth, 1);
			Assert.AreEqual(actual.PeriodYear, 2018);
			Assert.AreEqual(actual.TotalCostOfTraining, 99);
		}

		[TestCase(-11, -15, -20, true)]
		[TestCase(-10, -15, -20, true)]
		[TestCase(-9, -15, -20, false)]
		[TestCase(-5, -15, -20, false)]
		public void Is_Aggregation_Allowed(int firstDelay, int secondDelay, int thirdDelay, bool expected)
		{
			var payments = new List<Payment>
			{
				new Payment
				{
					ReceivedTime = DateTime.Now.AddMinutes(firstDelay)
				},
				new Payment
				{
					ReceivedTime = DateTime.Now.AddMinutes(secondDelay)
				},
				new Payment
				{
					ReceivedTime = DateTime.Now.AddMinutes(thirdDelay)
				}
			};

			var actual = _service.IsAggregationAllowed(payments);

			Assert.AreEqual(actual, expected);

		}
	}
}
