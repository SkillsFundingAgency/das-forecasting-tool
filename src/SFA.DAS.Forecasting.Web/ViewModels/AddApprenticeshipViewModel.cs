using SFA.DAS.Forecasting.Models.Estimation;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Domain.Shared.Validation;
using SFA.DAS.Forecasting.Web.ViewModels.Validation;
using FluentValidation.Attributes;

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
    }
}