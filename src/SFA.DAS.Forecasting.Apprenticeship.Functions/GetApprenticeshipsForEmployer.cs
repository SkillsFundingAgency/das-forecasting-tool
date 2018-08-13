using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Apprenticeship.Functions.Application;
using SFA.DAS.Forecasting.Functions.Framework;

using ApiApprenticeship = SFA.DAS.Commitments.Api.Types.Apprenticeship.Apprenticeship;

namespace SFA.DAS.Forecasting.Apprenticeship.Functions
{
    public class GetApprenticeshipsForEmployer : IFunction
    {
        private static IApprenticeshipCourseDataService _apprenticeshipCourseDataService;

        [FunctionName("GetApprenticeshipsForEmployer")]
        public static async Task Run(
            [QueueTrigger(QueueNames.GetApprenticeshipsForEmployer)]GetApprenticesihpMessage message,
            [Queue(QueueNames.StoreApprenticeships)] ICollector<ApprenticeshipMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<GetApprenticeshipHttpTrigger>(writer, executionContext,
               async (container, logger) =>
               {
                   logger.Debug($"Getting apprenticeships for employer {message.EmployerId}...");
                   var employerCommitmentsApi = container.GetInstance<IEmployerCommitmentApi>();
                   var apprenticeshipValidation = new ApprenticeshipValidation();
                   var mapper = new Mapper(container.GetInstance<IApprenticeshipCourseDataService>());

                   IEnumerable<ApiApprenticeship> apiApprenticeships = 
                      await employerCommitmentsApi.GetEmployerApprenticeships(message.EmployerId)
                      ?? new List<ApiApprenticeship>();
                   IEnumerable<long> failedValidation;

                   (apiApprenticeships, failedValidation) = apprenticeshipValidation.BusinessValidation(apiApprenticeships);
                   logger.Info($"{failedValidation.Count()} apprenticeships failed business validation");

                   (apiApprenticeships, failedValidation)  = apprenticeshipValidation.InputValidation(apiApprenticeships);
                   logger.Info($"{failedValidation.Count()} apprenticeships failed input validation");

                   var apprenticeships = apiApprenticeships.Select(mapper.Map);

                   logger.Info($"Sending {apprenticeships.Count()} apprenticeships for storing. EmployerId: {message.EmployerId} ");

                   foreach (var apprenticeship in apprenticeships)
                   {
                       outputQueueMessage.Add(await apprenticeship);
                   }
               });
        }
    }
}
