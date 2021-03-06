﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.PreLoad.Functions.Models;
using SFA.DAS.HashingService;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    public class AllLevyDeclarationsNoProjectionPreLoadHttpFunction : IFunction
    {
        [FunctionName("AllLevyDeclarationsNoProjectionPreLoadHttpFunction")]
        public static async Task<string> Run(
            [HttpTrigger(AuthorizationLevel.Function,
            "post", Route = "AllLevyDeclarationsNoProjectionPreLoadHttpFunction")]HttpRequestMessage req,
            [Queue(QueueNames.LevyPreLoadRequestNoProjection)] ICollector<PreLoadRequest> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer
            )
        {
            return await FunctionRunner.Run<AllLevyDeclarationsNoProjectionPreLoadHttpFunction, string>(writer, executionContext,
                async (container, logger) => 
                {
                    var body = await req.Content.ReadAsStringAsync();
                    var preLoadRequest = JsonConvert.DeserializeObject<PreLoadAllEmployersRequest>(body);

                    var levyDataService = container.GetInstance<IEmployerDataService>();
                    var hashingService = container.GetInstance<IHashingService>();

                    var employerIds = await levyDataService.EmployersForPeriod(preLoadRequest.PeriodYear, preLoadRequest.PeriodMonth);

                    logger.Info($"Found {employerIds.Count} employer(s) for period; Year: {preLoadRequest.PeriodYear} Month: {preLoadRequest.PeriodMonth}");

                    foreach(var id in employerIds)
                    {
                        outputQueueMessage.Add(
                            new PreLoadRequest
                            {
                                EmployerAccountIds = new[] { hashingService.HashValue(id) },
                                PeriodYear = preLoadRequest.PeriodYear,
                                PeriodMonth = preLoadRequest.PeriodMonth,
                            });
                    }

                    return $"Created {employerIds.Count} {nameof(PreLoadRequest)} messages";
                });
        }
    }
}
