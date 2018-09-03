using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Apprenticeship.Functions.Application;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Apprenticeship.Functions
{
    public class GetApprenticeshipsForEmployer : IFunction
    {
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

                   var apiApprenticeships = 
                      await employerCommitmentsApi.GetEmployerApprenticeships(
                          message.EmployerId, new ApprenticeshipSearchQuery
                          {
                              ApprenticeshipStatuses = new List<ApprenticeshipStatus>
                              {
                                  ApprenticeshipStatus.WaitingToStart,
                                  ApprenticeshipStatus.Live
                              }
                          })
                      ?? new ApprenticeshipSearchResponse();

                   var pageNumber = 2;

                   var apprenticeships = apiApprenticeships.Apprenticeships.ToList();

                   if (apiApprenticeships.TotalPages > 1)
                   {
                       while (pageNumber != apiApprenticeships.TotalPages)
                       {

                           var pageOfApprenticeshipSearchResponse =
                               await employerCommitmentsApi.GetEmployerApprenticeships(
                                   message.EmployerId, new ApprenticeshipSearchQuery
                                   {
                                       ApprenticeshipStatuses = new List<ApprenticeshipStatus>
                                       {
                                           ApprenticeshipStatus.WaitingToStart,
                                           ApprenticeshipStatus.Live
                                       },
                                       PageNumber = pageNumber
                                   });

                           apprenticeships.AddRange(pageOfApprenticeshipSearchResponse.Apprenticeships);

                           pageNumber++;
                       }
                       
                   }

                   IEnumerable<long> failedValidation;

                   (apprenticeships, failedValidation) = apprenticeshipValidation.BusinessValidation(apprenticeships);
                   logger.Info($"{failedValidation.Count()} apprenticeships failed business validation");

                   (apprenticeships, failedValidation)  = apprenticeshipValidation.InputValidation(apprenticeships);
                   logger.Info($"{failedValidation.Count()} apprenticeships failed input validation");

                   var mappeApprenticeships = apprenticeships.Select(mapper.Map).ToList();

                   logger.Info($"Sending {mappeApprenticeships.Count()} apprenticeships for storing. EmployerId: {message.EmployerId} ");

                   foreach (var apprenticeship in mappeApprenticeships)
                   {
                       outputQueueMessage.Add(await apprenticeship);
                   }
               });
        }
    }
}
