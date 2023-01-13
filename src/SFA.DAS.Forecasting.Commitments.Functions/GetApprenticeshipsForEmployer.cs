using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Commitments.Functions.Application;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class GetApprenticeshipsForEmployer 
    {
        private readonly IApprovalsService _approvalsService;

        public GetApprenticeshipsForEmployer(IApprovalsService approvalsService)
        {
            _approvalsService = approvalsService;
        }
        [FunctionName("GetApprenticeshipsForEmployer")]
        public async Task Run(
            [QueueTrigger(QueueNames.RefreshApprenticeshipsForEmployer)] RefreshApprenticeshipForAccountMessage message,
            [Queue(QueueNames.StoreApprenticeships)] ICollector<ApprenticeshipMessage> outputQueueMessage,
            ILogger logger)
        {
            
                   logger.LogInformation($"Getting apprenticeships for employer {message.EmployerId}...");
                   
                   var apprenticeshipValidation = new ApprenticeshipValidation();
                   var mapper = new Mapper();

                   var apprenticeships = await _approvalsService.GetApprenticeships(message.EmployerId);

                   IEnumerable<long> failedValidation;

                   (apprenticeships, failedValidation) = apprenticeshipValidation.BusinessValidation(apprenticeships);
                   logger.LogInformation($"{failedValidation.Count()} apprenticeships failed business validation");

                   var mappedApprenticeships = apprenticeships.Select(y => mapper.Map(y, message.EmployerId)).ToList();

                   logger.LogInformation($"Sending {mappedApprenticeships.Count} apprenticeships for storing. EmployerId: {message.EmployerId} ");

                   foreach (var apprenticeship in mappedApprenticeships)
                   {
                       outputQueueMessage.Add(apprenticeship);
                   }
        }
    }
}
