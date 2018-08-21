using SFA.DAS.Forecasting.Web.Extensions;
using System.Collections.Generic;

namespace SFA.DAS.Forecasting.Web.ViewModels.Validation
{
    public class AddApprenticeshipViewModelValidator : AddEditApprenticeshipViewModelValidator<AddApprenticeshipViewModel>
    {
        public Dictionary<string, string> ValidateAdd(AddApprenticeshipViewModel vm)
        {
            var dict = new Dictionary<string, string>();

            if (vm.TotalCostAsString.ToDecimal() < 1)
                dict.Add($"{nameof(vm.TotalCostAsString)}", "The total training cost was not entered");

            if (vm.Course == null)
            {
                dict.Add($"{nameof(vm.Course)}", "You must choose 1 apprenticeship");
            }
            else
            {
                if (!CheckTotalCost(vm, vm.TotalCostAsString))
                    dict.Add($"{nameof(vm.TotalCostAsString)}", "The total cost can't be higher than the total government funding band maximum for this apprenticeship");

                if (vm.IsTransferFunded == "on" && vm.Course.CourseType == Models.Estimation.ApprenticeshipCourseType.Framework)
                    dict.Add($"{nameof(vm.IsTransferFunded)}", "You can only fund Standards with your transfer allowance");
            }

            return dict;
        }
    }
}