using SFA.DAS.Forecasting.Models.Estimation;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Domain.Shared.Validation;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class AddApprenticeshipViewModel
    {
        public string Name { get; set; }
        public IEnumerable<ApprenticeshipCourse> AvailableApprenticeships { get; set; }

        public ApprenticeshipToAdd ApprenticeshipToAdd { get; set; }

        public string PreviousCourseId { get; set; }
      
        public List<ValidationResult> ValidationResults { get; set; }

        public IEnumerable<SelectListItem> ApprenticeshipList()
        {
	        var res = (from availableApprenticeship in AvailableApprenticeships
		        let text = availableApprenticeship.CourseType == ApprenticeshipCourseType.Standard ? $"{availableApprenticeship.Title}, Level: {availableApprenticeship.Level} (Standard)" : $"{availableApprenticeship.Title}, Level: {availableApprenticeship.Level}"
		        select new SelectListItem
		        {
			        Value = availableApprenticeship.Id,
			        Text = text
		        }).ToList();

            res.Insert(0, new SelectListItem { Selected = true, Value = "", Text = "Select one" } );
            return  res;
        }
    }
}