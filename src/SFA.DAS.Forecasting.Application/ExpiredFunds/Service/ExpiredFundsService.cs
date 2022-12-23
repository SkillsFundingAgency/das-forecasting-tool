using System;
using SFA.DAS.EmployerFinance.Types.Models;
using SFA.DAS.Forecasting.Domain.Levy;
using SFA.DAS.Forecasting.Models.Projections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Levy.Services;
using SFA.DAS.Forecasting.Domain.Payments.Services;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.Forecasting.Models.Estimation;
using CalendarPeriod = SFA.DAS.EmployerFinance.Types.Models.CalendarPeriod;

namespace SFA.DAS.Forecasting.Application.ExpiredFunds.Service
{
    public interface IExpiredFundsService
    {
        Task<Dictionary<CalendarPeriod, decimal>> GetExpiringFunds(IList<AccountProjectionModel> projections,
            long employerAccountId, ProjectionSource messageProjectionSource, DateTime projectionDate);

       	Task<Dictionary<CalendarPeriod, decimal>> GetExpiringFunds(ReadOnlyCollection<AccountEstimationProjectionModel> estimationProjectorProjections, 
	           long employerAccountId, ProjectionGenerationType messageProjectionSource, DateTime projectionDate);

    }
    public class ExpiredFundsService : IExpiredFundsService
    {
        private readonly IExpiredFunds _expiredFunds;
        private readonly ILevyDataSession _levyDataSession;
        private readonly IEmployerPaymentDataSession _employerPaymentDataSession;

        public ExpiredFundsService(IExpiredFunds expiredFunds, ILevyDataSession levyDataSession, IEmployerPaymentDataSession employerPaymentDataSession)
        {
            _expiredFunds = expiredFunds;
            _levyDataSession = levyDataSession;
            _employerPaymentDataSession = employerPaymentDataSession;
        }

        public Dictionary<CalendarPeriod, decimal> GetExpiringFunds(IList<AccountProjectionModel> projections, IEnumerable<LevyPeriod> levyPeriodTotals, Dictionary<CalendarPeriod,decimal> paymentsTotals, ProjectionSource messageProjectionSource, DateTime projectionDate)
        {
            var previousLevyTotals =
                levyPeriodTotals.ToDictionary(k => new CalendarPeriod(k.CalendarYear, k.CalendarMonth),
                    v => v.TotalNetLevyDeclared);
            
            var fundsIn = projections
                .ToDictionary(d => new CalendarPeriod(d.Year, d.Month), v => v.LevyFundsIn);
            
            var fundsOut = projections
                .OrderBy(c => c.Year)
                .ThenBy(c => c.Month)
                .Skip(ShouldSkipFirstMonthPaymentValue(projectionDate, messageProjectionSource))
                .ToDictionary(d => new CalendarPeriod(d.Year, d.Month),
                    v => v.LevyFundedCostOfTraining + v.LevyFundedCompletionPayments 
                        + v.TransferOutCostOfTraining + v.TransferOutCompletionPayments);

            fundsIn = fundsIn.Concat(previousLevyTotals).GroupBy(g => g.Key).ToDictionary(t => t.Key, t => t.Last().Value);
            
            fundsOut = fundsOut.Concat(paymentsTotals).GroupBy(g => g.Key).ToDictionary(t => t.Key, t => t.Last().Value);

            var expiringFunds = _expiredFunds.GetExpiringFunds(fundsIn, fundsOut,null,24);
            return (Dictionary<CalendarPeriod, decimal>) expiringFunds;
        }

        public Dictionary<CalendarPeriod, decimal> GetExpiringFunds(ReadOnlyCollection<AccountEstimationProjectionModel> estimationProjectorProjections, IEnumerable<LevyPeriod> levyPeriodTotals, Dictionary<CalendarPeriod, decimal> paymentsTotals, ProjectionGenerationType messageProjectionSource, DateTime projectionDate)
        {
            var previousLevyTotals =
                levyPeriodTotals.ToDictionary(k => new CalendarPeriod(k.CalendarYear, k.CalendarMonth),
                    v => v.TotalNetLevyDeclared);


            var fundsIn = estimationProjectorProjections.ToDictionary(d => new CalendarPeriod(d.Year, d.Month), v => v.FundsIn);

            var fundsOut = estimationProjectorProjections.OrderBy(c => c.Year)
                .ThenBy(c => c.Month)
                .Skip(ShouldSkipFirstMonthPaymentValue(projectionDate,(ProjectionSource) messageProjectionSource))
                .ToDictionary(d => new CalendarPeriod(d.Year, d.Month),
                v => v.AllModelledCosts.FundsOut + v.ActualCosts.FundsOut);
            
            fundsIn = fundsIn.Concat(previousLevyTotals).GroupBy(g => g.Key).ToDictionary(t => t.Key, t => t.Last().Value);

            fundsOut = fundsOut.Concat(paymentsTotals).GroupBy(g => g.Key).ToDictionary(t => t.Key, t => t.Last().Value);

            var expiringFunds = _expiredFunds.GetExpiringFunds(fundsIn, fundsOut, null, 24);

            return (Dictionary<CalendarPeriod, decimal>)expiringFunds;
        }

        public async Task<Dictionary<CalendarPeriod, decimal>> GetExpiringFunds(ReadOnlyCollection<AccountEstimationProjectionModel> estimationProjectorProjections, long employerAccountId, ProjectionGenerationType messageProjectionSource, DateTime projectionDate)
        {
            var netLevyTotal = await _levyDataSession.GetAllNetTotals(employerAccountId);
            var paymentsTotal = await _employerPaymentDataSession.GetPaymentTotals(employerAccountId);
            
            var expiringFunds = GetExpiringFunds(estimationProjectorProjections, netLevyTotal, paymentsTotal,messageProjectionSource,projectionDate);

            return expiringFunds;
        }

        private static int ShouldSkipFirstMonthPaymentValue(DateTime periodStart, ProjectionSource projectionGenerationType)
        {
            var result =  projectionGenerationType == ProjectionSource.LevyDeclaration && periodStart.Day < 19;

            return result ? 0 : 1;
        }

        public async Task<Dictionary<CalendarPeriod, decimal>> GetExpiringFunds(
            IList<AccountProjectionModel> projectionModels, long employerAccountId,
            ProjectionSource messageProjectionSource, DateTime projectionDate)
        {
            
            var netLevyTotal = await _levyDataSession.GetAllNetTotals(employerAccountId);
            var paymentsTotal = await _employerPaymentDataSession.GetPaymentTotals(employerAccountId);
            var expiringFunds = GetExpiringFunds(projectionModels, netLevyTotal, paymentsTotal, messageProjectionSource, projectionDate);
            

            return expiringFunds;
        }

       
    }
}
