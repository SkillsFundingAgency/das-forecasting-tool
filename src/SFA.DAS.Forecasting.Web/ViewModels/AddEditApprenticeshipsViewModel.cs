using Newtonsoft.Json;
using SFA.DAS.Forecasting.Models.Estimation;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using SFA.DAS.Forecasting.Web.Extensions;
using Microsoft.Azure.Documents.SystemFunctions;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class AddEditApprenticeshipsViewModel
    {
        private string _fundingPeriodJson;
        public List<ApprenticeshipCourse> Courses { get; set; } = new List<ApprenticeshipCourse>();
        public int? NumberOfApprentices { get; set; }
        public short? TotalInstallments { get; set; }
        public int? StartDateMonth { get; set; }
        public int? StartDateYear { get; set; }
        public string TotalCostAsString { get; set; }
        public decimal? CalculatedTotalCap => FundingCapCalculated * NumberOfApprentices;
        public string EstimationName { get; set; }
        public string HashedAccountId { get; set; }
        public string ApprenticeshipsId { get; set; }
        public ApprenticeshipCourse Course { get; set; } = new ApprenticeshipCourse();
        public string IsTransferFunded { get; set; }

        private IList<FundingPeriodViewModel> FundingBands => Course?.FundingPeriods?.Select(m => new FundingPeriodViewModel
                                                                                                {
                                                                                                    FromDate = m.EffectiveFrom,
                                                                                                    ToDate = m.EffectiveTo,
                                                                                                    FundingCap = m.FundingCap
                                                                                                }).ToList();
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

                         Selected = course.Id == Course?.Id
                     }).ToList();
            }
        }
        public DateTime StartDate
        {
            get
            {
                string[] format = { "yyyy-M-dd" };
                if (!DateTime.TryParseExact($"{StartDateYear}-{StartDateMonth}-01", 
                           format,
                           System.Globalization.CultureInfo.InvariantCulture,
                           System.Globalization.DateTimeStyles.None, 
                           out var startDate))
                {
                    return DateTime.MinValue;
                }

                return startDate;
            }
        }

        public decimal FundingCapCalculated => GetFundingPeriod().FundingCap;

        public FundingPeriodViewModel GetFundingPeriod()
        {
            if (Course?.FundingPeriods == null)
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

                return Course?.Id != null ? _fundingPeriodJson : null;
            }

        }
        
        public Dictionary<string, string> ValidateAdd(AddEditApprenticeshipsViewModel vm)
        {
            var dict = new Dictionary<string, string>();

            if (vm.Course == null)
            {
                dict.Add($"{nameof(vm.Course)}", "You must choose 1 apprenticeship");
            }
            else
            {
                if (vm.IsTransferFunded == "on" && vm.Course.CourseType == Models.Estimation.ApprenticeshipCourseType.Framework)
                    dict.Add($"{nameof(vm.IsTransferFunded)}", "You can only fund Standards with your transfer allowance");
            }

            if (vm.NumberOfApprentices <= 0 || !vm.NumberOfApprentices.HasValue)
            {
                dict.Add($"{nameof(vm.NumberOfApprentices)}", "You must enter more than zero apprentices");
            }

            if (vm.TotalInstallments < 12 || !vm.TotalInstallments.HasValue)
            {
                dict.Add($"{nameof(vm.TotalInstallments)}", "You must enter a minimum of twelve months");
            }

            if (
                vm.StartDateYear.ToString().Length < 4 || 
                vm.StartDate < DateTime.Now.AddMonths(-1) ||
                !vm.StartDateYear.HasValue || 
                !vm.StartDateMonth.HasValue
            )
            {
                dict.Add($"{nameof(vm.StartDateYear)}", "The start date cannot be in the past");
            }

            if (vm.TotalCostAsString.ToDecimal() <= 0)
            {
                dict.Add($"{nameof(vm.TotalCostAsString)}", "You must enter a total cost that is above zero");
            }

            return dict;
        }
    }
}