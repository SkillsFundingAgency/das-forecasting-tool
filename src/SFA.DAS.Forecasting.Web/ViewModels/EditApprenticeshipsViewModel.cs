using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Attributes;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Web.Extensions;
using SFA.DAS.Forecasting.Web.ViewModels.Validation;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    [Validator(typeof(EditApprenticeshipsViewModelValidator))]
    public class EditApprenticeshipsViewModel
    {
        public string ApprenticeshipsId { get; set; }
        public string EstimationName { get; set; }
        public string HashedAccountId { get; set; }
        public string CourseTitle { get; set; }
        public int Level { get; set; }
        public int NumberOfApprentices { get; set; }

        public short TotalInstallments { get; set; }
        public string TotalCostAsString { get; set; }

        public string FundingPeriodsJson { get; set; }
        public string CourseId { get; set; }

        public DateTime StartDate
        {
            get
            {
                DateTime.TryParse($"{StartDateYear}-{StartDateMonth}-1", out var startDate);
                return startDate;
            }
        }

        public int StartDateMonth { get; set; }
        public int StartDateYear { get; set; }

        public decimal? CalculatedTotalCap => GetFundingPeriod().FundingCap * NumberOfApprentices;

        public decimal FundingCapCalculated => GetFundingPeriod().FundingCap;

        public FundingPeriodViewModel GetFundingPeriod()
        {
            var fundingBands = JsonConvert.DeserializeObject<IList<FundingPeriodViewModel>>(FundingPeriodsJson);
            var fundingBand = fundingBands.FirstOrDefault(m =>
                   m.FromDate < StartDate
                && m.ToDate == null || m.ToDate > StartDate);

            if (fundingBand == null)
                fundingBand = fundingBands.Last();

            return fundingBand;
        }
    }
}