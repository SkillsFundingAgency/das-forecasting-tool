using FluentValidation.Attributes;
using SFA.DAS.Forecasting.Web.ViewModels.Validation;
using System;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.ViewModels
{

    [Validator(typeof(EditApprenticeshipsViewModelValidator))]
    public class EditApprenticeshipsViewModel : AddEditApprenticeshipsViewModel
    {
        public EditApprenticeshipsViewModel()
        {
        }
        
    }
}