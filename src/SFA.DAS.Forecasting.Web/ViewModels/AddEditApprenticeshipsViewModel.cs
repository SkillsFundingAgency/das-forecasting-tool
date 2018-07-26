using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class AddEditApprenticeshipsViewModel
    {
        public AddEditApprenticeshipsViewModel()
        {
        }

        public int NumberOfApprentices { get; set; }
        public short TotalInstallments { get; set; }
        public int StartDateMonth { get; set; }
        public int StartDateYear { get; set; }
        public string TotalCostAsString { get; set; }

        public DateTime StartDate
        {
            get
            {
                DateTime.TryParse($"{StartDateYear}-{StartDateMonth}-1", out var startDate);
                return startDate;
            }
        }

        public decimal FundingCapCalculated => GetFundingPeriod().FundingCap;

        public FundingPeriodViewModel GetFundingPeriod()
        {
            if (FundingPeriodsJson == null)
                return new FundingPeriodViewModel { FromDate = DateTime.MinValue, ToDate = DateTime.MaxValue, FundingCap = 0 };

            var fundingBands = JsonConvert.DeserializeObject<IList<FundingPeriodViewModel>>(FundingPeriodsJson);

            var fundingBand = fundingBands.FirstOrDefault(m =>
                   m.FromDate < StartDate
                && m.ToDate == null || m.ToDate > StartDate);

            if (fundingBand == null)
                fundingBand = fundingBands.Last();

            return fundingBand;
        }

        public string FundingPeriodsJson { get; set; }
    }
}