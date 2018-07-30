using SFA.DAS.Forecasting.Models.Estimation;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Web.ViewModels.Validation;
using FluentValidation.Attributes;
using Newtonsoft.Json;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    [Validator(typeof(AddApprenticeshipViewModelValidator))]
    public class AddApprenticeshipViewModel : AddEditApprenticeshipsViewModel
    {
        public List<ApprenticeshipCourse> Courses { get; set; } = new List<ApprenticeshipCourse>();
        public List<ValidationResult> ValidationResults { get; set; } = new List<ValidationResult>();

        public string CourseId { get; set; }

        public ApprenticeshipCourse Course { get; set; }

        // ToDo: Get Funding Period?
        public decimal CalculatedTotalCap => Course != null ? Course.FundingCap * NumberOfApprentices : 0;

        public IEnumerable<SelectListItem> ApprenticeshipCourses
        {
            get
            {
                return
                    Courses
                    .OrderBy(course => course.Title)
                    .Select(item => new SelectListItem { Value = item.Id, Text = item.Title, Selected = item.Id == CourseId })
                    .ToList();
            }
        }

        public override string FundingPeriodsJson {
            get
            {
                return Course != null ?
                    JsonConvert.SerializeObject(
                        Course.FundingPeriods
                            .Select(m => new FundingPeriodViewModel { FromDate = m.EffectiveFrom, ToDate = m.EffectiveTo, FundingCap = m.FundingCap }))
                    : null;
            }
        }

    }
}