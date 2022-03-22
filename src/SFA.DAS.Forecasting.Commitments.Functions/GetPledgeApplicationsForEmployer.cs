using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Commitments.Functions.Application;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class GetPledgeApplicationsForEmployer : IFunction
    {
        [FunctionName("GetPledgeApplicationsForEmployer")]
        public static async Task Run(
            [QueueTrigger(QueueNames.RefreshPledgeApplicationsForEmployer)] RefreshPledgeApplicationsForAccountMessage message,
            [Queue(QueueNames.StoreApprenticeships)] ICollector<ApprenticeshipMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<GetPledgeApplicationsForEmployer>(writer, executionContext,
               async (container, logger) =>
               {
                   logger.Info($"Getting pledges and applications for employer {message.EmployerId}...");
                   //var approvalsService = container.GetInstance<IApprovalsService>();
                   //var apprenticeshipValidation = new ApprenticeshipValidation();
                   //var mapper = new Mapper(container.GetInstance<IApprenticeshipCourseDataService>());

                   //var apprenticeships = await approvalsService.GetApprenticeships(message.EmployerId);

                   //IEnumerable<long> failedValidation;

                   //(apprenticeships, failedValidation) = apprenticeshipValidation.BusinessValidation(apprenticeships);
                   //logger.Info($"{failedValidation.Count()} apprenticeships failed business validation");

                   //var mappedApprenticeships = apprenticeships.Select(y => mapper.Map(y, message.EmployerId)).ToList();

                   //logger.Info($"Sending {mappedApprenticeships.Count} apprenticeships for storing. EmployerId: {message.EmployerId} ");

                   //foreach (var apprenticeship in mappedApprenticeships)
                   //{
                   //    outputQueueMessage.Add(await apprenticeship);
                   //}
               });
        }
    }
}
