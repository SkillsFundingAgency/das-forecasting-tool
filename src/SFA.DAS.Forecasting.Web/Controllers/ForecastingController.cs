using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using CsvHelper;
using SFA.DAS.Forecasting.Web.Mvc;
using SFA.DAS.Forecasting.Web.Orchestrators;

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
        [Route("", Name = "ForecastingBalance")]
        public async Task<ActionResult> Balance(string hashedAccountId)
        {
            var viewModel = await _orchestrator.Balance(hashedAccountId);
            return View(viewModel);
        }

        [HttpGet]
        [Route("visualisation", Name = "ForecastingVisualisation")]
        public async Task<ActionResult> Visualisation(string hashedAccountId)
        {
            var viewModel = await _orchestrator.Visualisation(hashedAccountId);

            return View(viewModel);
        }

        [HttpGet]
        [Route("download", Name = "DownloadCsv")]
        public async Task<ActionResult> Csv(string hashedAccountId)
        {
            var results = await _orchestrator.BalanceCsv(hashedAccountId);
            
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream))
                {
                    using (var csvWriter = new CsvWriter(streamWriter))
                    {
                        csvWriter.WriteRecords(results);
                        streamWriter.Flush();
                        memoryStream.Position = 0;
                        return File(memoryStream.ToArray(), "text/csv", $"esfaforecast_{DateTime.Now:yyyyMMddhhmmss}.csv");
                    }
                }
            }
        }
    }

}