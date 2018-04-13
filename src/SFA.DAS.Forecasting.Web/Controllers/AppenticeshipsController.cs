using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    //[ValidateMembership]
    //[AuthorizeForecasting]
    [RoutePrefix("accounts/{hashedAccountId}/forecasting")]
    public class AppenticeshipsController : Controller
    {

        private readonly IApprenticeshipOrchestrator _apprenticeshipOrchestrator;
        private readonly IMembershipService _membershipService;

              public AppenticeshipsController(IApprenticeshipOrchestrator apprenticeshipOrchestrator, IMembershipService membershipService)
        {
            _apprenticeshipOrchestrator = apprenticeshipOrchestrator;
            _membershipService = membershipService;
        }

       

      
    }
}