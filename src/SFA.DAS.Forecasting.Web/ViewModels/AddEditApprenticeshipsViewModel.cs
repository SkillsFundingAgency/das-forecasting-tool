using FluentValidation.Attributes;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Web.ViewModels.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    [Validator(typeof(AddEditApprenticeshipViewModelValidator))]
    public class AddEditApprenticeshipsViewModel
    {
        public AddEditApprenticeshipsViewModel()
        {
        }

        private string _fundingPeriodJson;
        public List<ApprenticeshipCourse> Courses { get; set; } = new List<ApprenticeshipCourse>();
        public int NumberOfApprentices { get; set; }
        public short TotalInstallments { get; set; }
        public int StartDateMonth { get; set; }
        public int StartDateYear { get; set; }
        public string TotalCostAsString { get; set; }
        public decimal? CalculatedTotalCap => FundingCapCalculated * NumberOfApprentices;
        public string EstimationName { get; set; }
        public string HashedAccountId { get; set; }
        public string ApprenticeshipsId { get; set; }
        public ApprenticeshipCourse Course { get; set; } = new ApprenticeshipCourse();
        public string IsTransferFunded { get; set; }

        private IList<FundingPeriodViewModel> FundingBands => Course.Id != null ? Course.FundingPeriods
                                                                                .Select(m => new FundingPeriodViewModel
                                                                                {
                                                                                    FromDate = m.EffectiveFrom,
                                                                                    ToDate = m.EffectiveTo,
                                                                                    FundingCap = m.FundingCap
                                                                                }).ToList() : null;
        public IEnumerable<SelectListItem> ApprenticeshipCourses
        {
            get
            {
                return

                    (from course in Courses
                        let text = course.CourseType == ApprenticeshipCourseType.Standard ? $"{course.Title}, Level: {course.Level} (Standard)" : $"{course.Title}, Level: {course.Level}"
                        select new SelectListItem
                        {
                            Value = course.Id,
                            Text = text,

                            Selected = course.Id == Course.Id
                        }).ToList();
            }
        }
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
            if (Course.FundingPeriods == null)
                return new FundingPeriodViewModel { FromDate = DateTime.MinValue, ToDate = DateTime.MaxValue, FundingCap = 0 };

          

            var fundingBand = FundingBands.FirstOrDefault(m =>
                                  m.FromDate < StartDate
                                  && m.ToDate == null || m.ToDate > StartDate) ?? FundingBands.Last();

            return fundingBand;
        }

        public string FundingPeriodsJson
        {
            get
            {
                _fundingPeriodJson = JsonConvert.SerializeObject(FundingBands);

                return Course.Id != null ? _fundingPeriodJson : null;
            }

        }
    }
}