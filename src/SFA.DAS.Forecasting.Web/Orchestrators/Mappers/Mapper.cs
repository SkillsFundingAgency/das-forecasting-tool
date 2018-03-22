using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Forecasting.ReadModel.Projections;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Orchestrators.Mappers
{
    public class Mapper
    {
        public IEnumerable<ProjectionsItemViewModel> MapProjections(IEnumerable<AccountProjectionReadModel> data)
        {
            return data.Select(x =>
                new ProjectionsItemViewModel
                {
                    Date = (new DateTime(x.Year, x.Month, 1)),
                    LevyCredit = x.FundsIn,
                    CostOfTraining = x.TotalCostOfTraining,
                    CompletionPayments = x.CompletionPayments,
                    ExpiredFunds = 0,
                    Balance = x.FutureFunds,
                    CoInvestmentEmployer = x.CoInvestmentEmployer,
                    CoInvestmentGovernment = x.CoInvestmentGovernment
                });
        }

        public ProjectionsCsvItemViewModel ToCsvProjections(ProjectionsItemViewModel x)
        {
            return new ProjectionsCsvItemViewModel
            {
                Date = x.Date.ToGdsFormatShortMonthWithoutDay(),
                LevyCredit = x.LevyCredit,
                CostOfTraining = x.CostOfTraining,
                CompletionPayments = x.CompletionPayments,
                CoInvestmentEmployer = x.CoInvestmentEmployer,
                CoInvestmentGovernment = x.CoInvestmentGovernment,
                Balance = x.Balance
            };

        }
        
    }
}