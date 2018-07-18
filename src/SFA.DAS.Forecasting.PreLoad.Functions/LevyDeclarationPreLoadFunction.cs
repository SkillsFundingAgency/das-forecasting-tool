using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Infrastructure.Telemetry;
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
	               var telemetry = container.GetInstance<IAppInsightsTelemetry>();

				   var hashingService = container.GetInstance<IHashingService>();

                   if (request == null)
                   {
                       var msg = $"{nameof(PreLoadRequest)} not valid. Function will exit.";
	                   telemetry.Warning("LevyDeclarationPreLoadFunction", msg, "FunctionRunner.Run", executionContext.InvocationId);

                       return;
                   }

                   if (request.SubstitutionId.HasValue && request.EmployerAccountIds.Count() != 1)
                   {
                       var msg = $"If {nameof(request.SubstitutionId)} is provided there must be exactly 1 EmployerAccountId";
					   telemetry.Warning("LevyDeclarationPreLoadFunction", msg, "FunctionRunner.Run", executionContext.InvocationId);

					   return;
                   }

                   var levyDataService = container.GetInstance<IEmployerDataService>();
				   telemetry.Info("LevyDeclarationPreLoadFunction", $"LevyDeclarationPreLoadHttpFunction started. Data: {string.Join("|", request.EmployerAccountIds)}, {request.PeriodYear}, {request.PeriodMonth}", "FunctionRunner.Run", executionContext.InvocationId);
                   var messageCount = 0;
                   var schemes = new Dictionary<string, string>();
                   foreach (var employerId in request.EmployerAccountIds)
                   {
                       var levyDeclarations = await levyDataService.LevyForPeriod(employerId, request.PeriodYear, request.PeriodMonth);
                       if (levyDeclarations.Count < 1)
                       {
	                       telemetry.Info("LevyDeclarationPreLoadFunction", $"No levy declarations found for employer: {employerId}", "FunctionRunner.Run", executionContext.InvocationId);
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
	               telemetry.Info("LevyDeclarationPreLoadFunction", $"Added {messageCount} levy declarations to  {QueueNames.ValidateLevyDeclaration} queue.", "FunctionRunner.Run");

                   if (request.SubstitutionId.HasValue)
                   {
	                   telemetry.Info("LevyDeclarationPreLoadFunction", $"Added message with SubstitutionID: {hashingService.HashValue(request.SubstitutionId.Value)}", "FunctionRunner.Run");
                   }

	               telemetry.Info("LevyDeclarationPreLoadFunction", $"Added {messageCount} levy declarations", "FunctionRunner.Run");
               });
        }
    }
}