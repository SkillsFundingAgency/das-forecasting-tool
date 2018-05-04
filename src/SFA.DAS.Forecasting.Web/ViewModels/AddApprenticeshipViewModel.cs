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
            var res = AvailableApprenticeships.Select(item => new SelectListItem {Value = item.Id, Text = item.Title}).ToList();

            res.Insert(0, new SelectListItem { Selected = true, Value = "", Text = "Select one" } );
            return  res;
        }
    }
}