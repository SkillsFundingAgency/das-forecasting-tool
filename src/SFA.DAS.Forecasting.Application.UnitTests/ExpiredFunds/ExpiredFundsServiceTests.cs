using System;
using AutoMoq;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Domain.ExpiredFunds;
using SFA.DAS.Forecasting.Application.ExpiredFunds.Service;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Models.Projections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using SFA.DAS.Forecasting.Models.Estimation;

namespace SFA.DAS.Forecasting.Application.UnitTests.ExpiredFunds
{

    [TestFixture]
    public class ExpiredFundsServiceTests
    {
        private AutoMoqer _moqer;
        private long employerAccountId = 12345;
        private IList<AccountProjectionModel> accountProjectionModels;
        private IList<AccountEstimationProjectionModel> _accountEstimationProjectionModels;
        private IList<LevyPeriod> netLevyTotals;
        private Dictionary<CalendarPeriod, decimal> paymentTotals;

        private Dictionary<CalendarPeriod, decimal> expiredFundsIn;
        private Dictionary<CalendarPeriod, decimal> expiredFundsOut;

        private Dictionary<CalendarPeriod, decimal> estimatedExpiredFundsIn;
        private Dictionary<CalendarPeriod, decimal> estimatedExpiredFundsOut;

        [SetUp]
        public void SetUp()
        {
            _moqer = new AutoMoqer();

            #region ActualProjectionSetup

            
            //ProjectionModel

            accountProjectionModels = new List<AccountProjectionModel>()
            {
                new AccountProjectionModel()
                {
                    EmployerAccountId = employerAccountId,
                    Month = 1,
                    Year = 2018,
                    LevyFundsIn = 1000,
                    FutureFunds = 200,
                    LevyFundedCompletionPayments = 0,
                    LevyFundedCostOfTraining = 800,
                    ProjectionGenerationType = ProjectionGenerationType.LevyDeclaration
                },
                new AccountProjectionModel()
                {
                    EmployerAccountId = employerAccountId,
                    Month =2,
                    Year = 2018,
                    LevyFundsIn = 1000,
                    FutureFunds = 700,
                    LevyFundedCompletionPayments = 0,
                    LevyFundedCostOfTraining = 500,
                    ProjectionGenerationType = ProjectionGenerationType.LevyDeclaration
                },
                new AccountProjectionModel()
                {
                    EmployerAccountId = employerAccountId,
                    Month = 3,
                    Year = 2018,
                    LevyFundsIn = 1000,
                    FutureFunds = -100,
                    LevyFundedCompletionPayments = 1000,
                    LevyFundedCostOfTraining = 800,
                    ProjectionGenerationType = ProjectionGenerationType.LevyDeclaration
                },

            };



            //NetLevyTotals Object

            netLevyTotals = new List<LevyPeriod>
            {
                new LevyPeriod(12345, "2016", 1,DateTime.Parse("2016-01-01"), 1000m, null),
                new LevyPeriod(12345, "2017", 10,DateTime.Parse("2017-10-01"), 500m, null),
                new LevyPeriod(12345, "2017", 11,DateTime.Parse("2017-11-01"), 800m, null),
                new LevyPeriod(12345, "2017", 12,DateTime.Parse("2017-12-01"), 300m, null)
            };
            //Payments Object
            paymentTotals = new Dictionary<CalendarPeriod, decimal>
            {
                {new CalendarPeriod(2016,1), 1000m  },
                {new CalendarPeriod(2017,10), 500m  },
                {new CalendarPeriod(2017,11), 800m  },
                {new CalendarPeriod(2017,12), 300m  },
            };
            //

            expiredFundsIn = new Dictionary<CalendarPeriod, decimal>
            {
                {new CalendarPeriod(2018,1), 1000m  },
                {new CalendarPeriod(2018,2), 1000m  },
                {new CalendarPeriod(2018,3), 1000m  },
                { new CalendarPeriod(2016,1), 1000m  },
                {new CalendarPeriod(2017,10), 500m  },
                {new CalendarPeriod(2017,11), 800m  },
                {new CalendarPeriod(2017,12), 300m  }

            };

            expiredFundsOut = new Dictionary<CalendarPeriod, decimal>
            {
                {new CalendarPeriod(2018,1), 800m  },
                {new CalendarPeriod(2018,2), 500m  },
                {new CalendarPeriod(2018,3), 1800m  },
                { new CalendarPeriod(2016,1), 1000m  },
                {new CalendarPeriod(2017,10), 500m  },
                {new CalendarPeriod(2017,11), 800m  },
                {new CalendarPeriod(2017,12), 300m  }

            };


            #endregion

            #region EsitmatedProjectionSetup

            _accountEstimationProjectionModels = new List<AccountEstimationProjectionModel>()
            {
                new AccountEstimationProjectionModel()
                {
                    Month = 1,
                    Year = 2018,
                    EstimatedProjectionBalance = 100,
                    FundsIn = 1000,
                    ActualCosts = new AccountEstimationProjectionModel.Cost()
                    {
                        LevyCompletionPayments = 0,
                        LevyCostOfTraining = 800
                    },
                    AllModelledCosts = new AccountEstimationProjectionModel.Cost()
                    {
                        LevyCostOfTraining =  100,
                        LevyCompletionPayments = 0
                    }
                },
                new AccountEstimationProjectionModel()
                {
                    Month = 2,
                    Year = 2018,
                    EstimatedProjectionBalance = 400,
                    FundsIn = 1000,
                    ActualCosts = new AccountEstimationProjectionModel.Cost()
                    {
                        LevyCompletionPayments = 0,
                        LevyCostOfTraining = 500
                    },
                    AllModelledCosts = new AccountEstimationProjectionModel.Cost()
                    {
                        LevyCostOfTraining =  200,
                        LevyCompletionPayments = 0
                    }
                },
                new AccountEstimationProjectionModel()
                {
                    Month = 3,
                    Year = 2018,
                    EstimatedProjectionBalance = -1700,
                    FundsIn = 1000,
                    ActualCosts = new AccountEstimationProjectionModel.Cost()
                    {
                        LevyCompletionPayments = 1000,
                        LevyCostOfTraining = 800
                    },
                    AllModelledCosts = new AccountEstimationProjectionModel.Cost()
                    {
                        LevyCostOfTraining =  300,
                        LevyCompletionPayments = 1000
                    }
                }
            };


           
            estimatedExpiredFundsIn = new Dictionary<CalendarPeriod, decimal>
            {
                {new CalendarPeriod(2018,1), 1000m  },
                {new CalendarPeriod(2018,2), 1000m  },
                {new CalendarPeriod(2018,3), 1000m  },
                { new CalendarPeriod(2016,1), 1000m  },
                {new CalendarPeriod(2017,10), 500m  },
                {new CalendarPeriod(2017,11), 800m  },
                {new CalendarPeriod(2017,12), 300m  }

            };

            estimatedExpiredFundsOut = new Dictionary<CalendarPeriod, decimal>
            {
                {new CalendarPeriod(2018,1), 900m  },
                {new CalendarPeriod(2018,2), 700m  },
                {new CalendarPeriod(2018,3), 3100m  },
                { new CalendarPeriod(2016,1), 1000m  },
                {new CalendarPeriod(2017,10), 500m  },
                {new CalendarPeriod(2017,11), 800m  },
                {new CalendarPeriod(2017,12), 300m  }

            };
            #endregion
        }

        [Test]
        public async Task Get_Expired_Funds_By_AccountId_Retrieves_NetLevyTotals()
        {
            var sut = _moqer.Resolve<ExpiredFundsService>();

            var expiringFunds = sut.GetExpiringFunds(accountProjectionModels, employerAccountId);

            var levyDataSession = _moqer.GetMock<ILevyDataSession>();
            levyDataSession.Verify(v => v.GetAllNetTotals(employerAccountId));
        }

        [Test]
        public async Task Get_Expired_Funds_By_AccountId_Retrieves_PaymentTotals()
        {
            var sut = _moqer.Resolve<ExpiredFundsService>();

            var expiringFunds = sut.GetExpiringFunds(accountProjectionModels, employerAccountId);

            var employerPaymentDataSession = _moqer.GetMock<IEmployerPaymentDataSession>();
            employerPaymentDataSession.Verify(v => v.GetPaymentTotals(employerAccountId));
        }

        [Test]
        public async Task Get_Expired_Funds_By_AccountId_Calculates_ExpiredFunds()
        {

            var employerPaymentDataSession = _moqer.GetMock<IEmployerPaymentDataSession>();
            employerPaymentDataSession.Setup(s => s.GetPaymentTotals(12345)).ReturnsAsync(paymentTotals);

            var sut = _moqer.Resolve<ExpiredFundsService>();

            var expiringFunds = sut.GetExpiringFunds(accountProjectionModels, employerAccountId);

            var expiredFunds = _moqer.GetMock<IExpiredFunds>();
            expiredFunds.Verify(v => v.GetExpiringFunds(It.IsAny<Dictionary<CalendarPeriod, decimal>>(), It.IsAny<Dictionary<CalendarPeriod, decimal>>(), null, 24));
        }

        [Test]
        public async Task Get_Expired_Funds_Calculates_ExpiredFunds()
        {
            var sut = _moqer.Resolve<ExpiredFundsService>();
            var employerPaymentDataSession = _moqer.GetMock<IExpiredFunds>();

            Dictionary<CalendarPeriod, decimal> calledFundIn = null;
            Dictionary<CalendarPeriod, decimal> calledFundOut = null;
            Dictionary<CalendarPeriod, decimal> calledExpired = null;
            int calledMonths = 0;

            employerPaymentDataSession.Setup(s => s.GetExpiringFunds(It.IsAny<Dictionary<CalendarPeriod, decimal>>(), It.IsAny<Dictionary<CalendarPeriod, decimal>>(), null, 24))
                                        .Callback<Dictionary<CalendarPeriod, decimal>, Dictionary<CalendarPeriod, decimal>, Dictionary<CalendarPeriod, decimal>, int>(
                                                        (fundsIn, fundsOut, expired, months) =>
                                                        {
                                                            calledFundIn = fundsIn;
                                                            calledFundOut = fundsOut;
                                                            calledExpired = expired;
                                                            calledMonths = months;
                                                        });

            var expiringFunds = sut.GetExpiringFunds(accountProjectionModels, netLevyTotals, paymentTotals);


            calledFundIn.ShouldAllBeEquivalentTo(expiredFundsIn);    
            calledFundOut.ShouldAllBeEquivalentTo(expiredFundsOut);
            calledExpired.ShouldAllBeEquivalentTo(calledExpired);
            calledMonths.Should().Equals(24);

        }

        [Test]
        public async Task Get_Estiamted_Expired_Funds_Calculates_ExpiredFunds()
        {
            var sut = _moqer.Resolve<ExpiredFundsService>();
            var ExpiredFunds = _moqer.GetMock<IExpiredFunds>();

            Dictionary<CalendarPeriod, decimal> calledFundIn = null;
            Dictionary<CalendarPeriod, decimal> calledFundOut = null;
            Dictionary<CalendarPeriod, decimal> calledExpired = null;
            int calledMonths = 0;

            ExpiredFunds.Setup(s => s.GetExpiringFunds(It.IsAny<Dictionary<CalendarPeriod, decimal>>(), It.IsAny<Dictionary<CalendarPeriod, decimal>>(), null, 24))
                .Callback<Dictionary<CalendarPeriod, decimal>, Dictionary<CalendarPeriod, decimal>, Dictionary<CalendarPeriod, decimal>, int>(
                    (fundsIn, fundsOut, expired, months) =>
                    {
                        calledFundIn = fundsIn;
                        calledFundOut = fundsOut;
                        calledExpired = expired;
                        calledMonths = months;
                    });

            var expiringFunds = sut.GetExpiringFunds(_accountEstimationProjectionModels.ToList().AsReadOnly(), netLevyTotals, paymentTotals);


            calledFundIn.ShouldAllBeEquivalentTo(estimatedExpiredFundsIn);
            calledFundOut.ShouldAllBeEquivalentTo(estimatedExpiredFundsOut);
            calledExpired.ShouldAllBeEquivalentTo(calledExpired);
            calledMonths.Should().Equals(24);

        }





    }
}
