using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

using SFA.DAS.Forecasting.Web.Mvc;
using SFA.DAS.Forecasting.Web.Orchestrators;
using SFA.DAS.Forecasting.Web.ViewModels;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [ForecastingRoutePrefix("accounts/{hashedaccountId}/forecasting")]
    public class ForecastingController : Controller
    {
        private readonly ForecastingOrchestrator _orchestrator;

        public ForecastingController(ForecastingOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("balance", Name = "ForecastingBalance")]
        public async Task<ActionResult> Balance(string hashedAccountId)
        {
            var viewModel = await _orchestrator.Balance(hashedAccountId);
            return View(viewModel);
        }

        [HttpGet]
        [Route("apprenticeships", Name = "ForecastingApprenticeships")]
        public ActionResult Apprenticeships()
        {
            return View();
        }

        [HttpGet]
        [Route("visualisation", Name = "ForecastingVisualisation")]
        public ActionResult Visualisation()
        {
            var startMonth = new DateTime(2018, 01, 01);
            var viewModel = new VisualisationViewModel
            {
                ChartTitle = "Your 4 Year Forecast",
                ChartItems = new List<ChartItemViewModel>
                {
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(0), Amount = 980},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(1), Amount = 50},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(2), Amount = -100},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(3), Amount = 53},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(4), Amount = 12},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(5), Amount = 978},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(5), Amount = 976},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(7), Amount = 925},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(8), Amount = 750},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(9), Amount = 450},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(10), Amount = 325},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(11), Amount = 300},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(12), Amount = 198},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(13), Amount = 50},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(14), Amount = -10},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(15), Amount = 53},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(16), Amount = 12},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(17), Amount = 978},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(18), Amount = 976},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(19), Amount = 925},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(20), Amount = 750},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(21), Amount = 450},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(22), Amount = 325},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(23), Amount = 300},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(24), Amount = 15},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(25), Amount = 30},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(26), Amount = -25},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(27), Amount = 1},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(28), Amount = -10},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(29), Amount = 50},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(30), Amount = 505},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(31), Amount = 543},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(32), Amount = 234},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(33), Amount = 725},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(34), Amount = 75},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(35), Amount = 555},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(36), Amount = 150},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(37), Amount = 300},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(38), Amount = -243},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(39), Amount = 188},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(40), Amount = 43},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(41), Amount = 675},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(42), Amount = -135},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(43), Amount = 654},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(44), Amount = 345},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(45), Amount = -567},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(46), Amount = 35},
                    new ChartItemViewModel {BalanceMonth = startMonth.AddMonths(47), Amount = 671}
                }
            };

            return View(viewModel);
        }
    }
}