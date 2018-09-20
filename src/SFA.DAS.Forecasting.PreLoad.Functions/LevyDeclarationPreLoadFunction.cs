using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.PreLoad.Functions.Models;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationPreLoadFunction : IFunction
    {
        [FunctionName("LevyDeclarationPreLoadFunction")]
        public static async Task Run(
            [QueueTrigger(QueueNames.LevyPreLoadRequest)]PreLoadRequest request,
            [Queue(QueueNames.ValidateLevyDeclaration)] ICollector<LevySchemeDeclarationUpdatedMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<LevyDeclarationPreLoadFunction>(writer, executionContext,
               async (container, logger) =>
               {
                   var hashingService = container.GetInstance<IHashingService>();

                   if (request == null)
                   {
                       var msg = $"{nameof(PreLoadRequest)} not valid. Function will exit.";
                       logger.Warn(msg);
                       return;
                   }

                   if (request.SubstitutionId.HasValue && request.EmployerAccountIds.Count() != 1)
                   {
                       var msg = $"If {nameof(request.SubstitutionId)} is provided there must be exactly 1 EmployerAccountId";
                       logger.Warn(msg);
                       return;
                   }

                   var levyDataService = container.GetInstance<IEmployerDataService>();
                   logger.Info($"LevyDeclarationPreLoadHttpFunction started. Data: {string.Join("|", request.EmployerAccountIds)}, {request.PeriodYear}, {request.PeriodMonth}");
                   var messageCount = 0;
                   var schemes = new Dictionary<string, string>();
                   foreach (var employerId in request.EmployerAccountIds)
                   {
                       var levyDeclarations = await levyDataService.LevyForPeriod(employerId, request.PeriodYear, request.PeriodMonth);
                       if (!levyDeclarations.Any())
                       {
                           logger.Info($"No levy declarations found for employer: {employerId}");
                           continue;
                       }
                       levyDeclarations.ForEach(ld =>
                       {
                           messageCount++;
                           if (request.SubstitutionId.HasValue)
                           {
                               ld.AccountId = request.SubstitutionId.Value;
                               if (!schemes.ContainsKey(ld.EmpRef))
                                   schemes.Add(ld.EmpRef, Guid.NewGuid().ToString("N"));
                               ld.EmpRef = schemes[ld.EmpRef];
                           }
                           outputQueueMessage.Add(ld);
                       });
                   }

                   logger.Info($"Added {messageCount} levy declarations to  {QueueNames.ValidateLevyDeclaration} queue.");

                   if (request.SubstitutionId.HasValue)
                   {
                       logger.Info($"Added message with SubstitutionID: {hashingService.HashValue(request.SubstitutionId.Value)}");
                   }
                   logger.Info($"Added {messageCount} levy declarations");
               });
        }
    }
}