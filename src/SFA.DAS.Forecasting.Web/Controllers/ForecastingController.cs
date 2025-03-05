using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;
using SFA.DAS.Forecasting.Web.Authentication;
using SFA.DAS.Forecasting.Web.Configuration;
using SFA.DAS.Forecasting.Web.Orchestrators;

namespace SFA.DAS.Forecasting.Web.Controllers;

[Authorize(Policy = nameof(PolicyNames.HasEmployerAccount))]
[Route("accounts/{hashedaccountId}/forecasting")]
[SetNavigationSection(NavigationSection.AccountsFinance)]
public class ForecastingController : Controller
{
    private readonly IForecastingOrchestrator _orchestrator;

    public ForecastingController(IForecastingOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }
    
    [HttpGet]
    [Route("download", Name = RouteNames.DownloadCsv)]
    public async Task<ActionResult> Csv(string hashedAccountId)
    {
        var results = await _orchestrator.BalanceCsv(hashedAccountId);

        return CreateCsvStream(results, "esfaforecast");
    }

    [HttpGet]
    [Route("download-apprenticeships", Name = RouteNames.DownloadApprenticeships)]
    public async Task<ActionResult> DownloadApprenticeshipDetailsCsv(string hashedAccountId)
    {
        var results = await _orchestrator.ApprenticeshipsCsv(hashedAccountId);

        return CreateCsvStream(results, "esfa_apprenticeships");
    }

    [HttpGet]
    [Route("expired-funds-guidance", Name = RouteNames.ExpiredFundsGuidance)]
    public ActionResult ExpiredFundsGuidance()
    {
        return View();
    }

    private ActionResult CreateCsvStream<T>(IEnumerable<T> results, string fileNamePreFix)
    {
        using (var memoryStream = new MemoryStream())
        {
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture))
                {
                    csvWriter.WriteRecords(results);
                    streamWriter.Flush();
                    memoryStream.Position = 0;
                    return File(memoryStream.ToArray(), "text/csv", $"{fileNamePreFix}_{DateTime.Now:yyyyMMddhhmmss}.csv");
                }
            }
        }
    }
}