using System;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using CsvHelper;
using SFA.DAS.Forecasting.Web.Attributes;
using SFA.DAS.Forecasting.Web.Orchestrators;

namespace SFA.DAS.Forecasting.Web.Controllers
{
    [AuthorizeForecasting]
    [RoutePrefix("accounts/{hashedaccountId}/forecasting")]
    public class ForecastingController : Controller
    {
        private readonly ForecastingOrchestrator _orchestrator;

        public ForecastingController(ForecastingOrchestrator orchestrator)
        {
            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("projections", Name = "Forecastingprojections")]
        public async Task<ActionResult> Projections(string hashedAccountId)
        {
            var viewModel = await _orchestrator.Projections(hashedAccountId);
            return View(viewModel);
        }

        [HttpGet]
        [Route("projections/download", Name = "DownloadCsv")]
        public async Task<ActionResult> Csv(string hashedAccountId)
        {
            var results = await _orchestrator.ProjectionsCsv(hashedAccountId);
            
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