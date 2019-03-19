using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Levy.Messages.PreLoad;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.HashingService;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationPreLoadFunction : IFunction
    {
        [FunctionName("LevyDeclarationPreLoadFunction")]
        [return: Queue(QueueNames.GenerateProjections)]
        public static async Task<GenerateAccountProjectionCommand> Run(
            [QueueTrigger(QueueNames.LevyPreLoadRequest)]PreLoadLevyMessage request,
            [Queue(QueueNames.ValidateLevyDeclaration)] ICollector<LevySchemeDeclarationUpdatedMessage> outputQueueMessage,
			ExecutionContext executionContext,
            TraceWriter writer)
        {
            return await FunctionRunner.Run<LevyDeclarationPreLoadFunction, GenerateAccountProjectionCommand>(writer, executionContext,
               async (container, logger) =>
               {
                   var hashingService = container.GetInstance<IHashingService>();

                   if (request == null)
                   {
                       var msg = $"{nameof(PreLoadLevyMessage)} not valid. Function will exit.";
                       logger.Warn(msg);
                       return;
                   }
                   else if (request.SubstitutionId ==0  && request.EmployerAccountId == 0)
                   {
                       var msg = $"If {nameof(request.SubstitutionId)} is provided there must be exactly 1 EmployerAccountId";
                       logger.Warn(msg);
                       return null;
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
                           logger.Info($"No levy declarations found for employer {employerId} at the requested period - projection will be re-generated using last totals");
                           return new GenerateAccountProjectionCommand
                           {
                               EmployerAccountId = hashingService.DecodeValue(employerId),
                               ProjectionSource = ProjectionSource.LevyDeclaration
                           };
                       }
                       levyDeclarations.ForEach(ld =>
                       {
                           messageCount++;
                           if (request.SubstitutionId != 0)
                           {
                               ld.AccountId = request.SubstitutionId;
                               if (!schemes.ContainsKey(ld.EmpRef))
                                   schemes.Add(ld.EmpRef, Guid.NewGuid().ToString("N"));
                               ld.EmpRef = schemes[ld.EmpRef];
                           }
                           outputQueueMessage.Add(ld);
                       });


                       logger.Info($"Added {messageCount} levy declarations to  {QueueNames.ValidateLevyDeclaration} queue.");

                       if (request.SubstitutionId != 0)
                       {
                           logger.Info($"Added message with SubstitutionID: {hashingService.HashValue(request.SubstitutionId)}");
                       }
                       logger.Info($"Added {messageCount} levy declarations");
                   }
                   return null;
               });
        }
    }
}