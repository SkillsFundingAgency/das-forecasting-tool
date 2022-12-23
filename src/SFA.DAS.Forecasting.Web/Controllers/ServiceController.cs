using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Forecasting.Web.Mvc;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [ForecastingRoutePrefix("Service")]
    public class ServiceController : Controller
    {

        [Route("signout")]
        public async Task<IActionResult> SignOutEmployer()
        {
            return SignOut(); //TODO FAI-625
        }

    }
}