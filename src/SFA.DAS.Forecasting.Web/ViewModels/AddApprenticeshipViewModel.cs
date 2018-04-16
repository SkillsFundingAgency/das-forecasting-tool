﻿using SFA.DAS.Forecasting.Models.Estimation;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.ViewModels
{
    public class AddApprenticeshipViewModel
    {
        public string Name { get; set; }
        public IEnumerable<ApprenticeshipCourse> AvailableApprenticeships { get; set; }

        public string CourseId { get; set; }
        public ApprenticeshipToAdd ApprenticeshipToAdd { get; set; }

        public IEnumerable<SelectListItem> ApprenticeshipList()
        {
            var res = AvailableApprenticeships.Select(item => new SelectListItem {Value = item.Id, Text = item.Title}).ToList();

            res.Insert(0, new SelectListItem { Selected = true, Value = "noselection", Text = "Select one" } );
            return  res;
        }
    }
}