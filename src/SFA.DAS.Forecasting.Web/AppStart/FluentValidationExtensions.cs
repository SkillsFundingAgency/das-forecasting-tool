using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Forecasting.Web.ViewModels;
using SFA.DAS.Forecasting.Web.ViewModels.Validation;

namespace SFA.DAS.Forecasting.Web;

public static class FluentValidationExtensions
{
    public static void AddFluentValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<AddEditApprenticeshipsViewModel>, AddEditApprenticeshipViewModelValidator>();
    }
}