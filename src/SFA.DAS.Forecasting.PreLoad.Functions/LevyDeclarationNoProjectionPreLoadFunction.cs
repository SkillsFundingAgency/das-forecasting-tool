using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Levy.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.PreLoad.Functions.Models;

namespace SFA.DAS.Forecasting.PreLoad.Functions
{
    [StorageAccount("StorageConnectionString")]
    public class LevyDeclarationNoProjectionPreLoadFunction
    {
	    private readonly IEmployerDataService _employerDataService;

	    public LevyDeclarationNoProjectionPreLoadFunction(IEmployerDataService employerDataService)
	    {
		    _employerDataService = employerDataService;
	    }
        [FunctionName("LevyDeclarationNoProjectionPreLoadFunction")]
        public async Task Run(
            [QueueTrigger(QueueNames.LevyPreLoadRequestNoProjection)]PreLoadRequest request,
            [Queue(QueueNames.ValidateLevyDeclarationNoProjection)] ICollector<LevySchemeDeclarationUpdatedMessage> outputNoProjectionQueueMessage,
			ExecutionContext executionContext,
            ILogger logger)
        {
		   if (request == null)
		   {
			   var msg = $"{nameof(PreLoadRequest)} not valid. Function will exit.";
			   logger.LogWarning(msg);
			   return;
		   }

		   if (request.SubstitutionId.HasValue && request.EmployerAccountIds.Count() != 1)
		   {
			   var msg = $"If {nameof(request.SubstitutionId)} is provided there must be exactly 1 EmployerAccountId";
			   logger.LogWarning(msg);
			   return;
		   }

		   logger.LogInformation($"LevyDeclarationPreLoadHttpFunction started. Data: {string.Join("|", request.EmployerAccountIds)}, {request.PeriodYear}, {request.PeriodMonth}");
		   var messageCount = 0;
		   var schemes = new Dictionary<string, string>();
		   foreach (var employerId in request.EmployerAccountIds)
		   {
			   var levyDeclarations = await _employerDataService.LevyForPeriod(employerId, request.PeriodYear, request.PeriodMonth);
			   if (!levyDeclarations.Any())
			   {
				   logger.LogInformation($"No levy declarations found for employer {employerId} at the requested period");
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
				   outputNoProjectionQueueMessage.Add(ld);
			   });
		   }

		   logger.LogInformation($"Added {messageCount} levy declarations to  {QueueNames.ValidateLevyDeclaration} queue.");

		   if (request.SubstitutionId.HasValue)
		   {
			   logger.LogInformation($"Added message with SubstitutionID: {request.SubstitutionId.Value}");
		   }
		   logger.LogInformation($"Added {messageCount} levy declarations");
			   
		}
    }
}