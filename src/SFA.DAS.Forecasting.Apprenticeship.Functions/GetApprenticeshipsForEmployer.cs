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
using SFA.DAS.Forecasting.Commitments.Functions.Application;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class GetApprenticeshipsForEmployer : IFunction
    {
        [FunctionName("GetApprenticeshipsForEmployer")]
        public static async Task Run(
            [QueueTrigger(QueueNames.RefreshApprenticeshipsForEmployer)]RefreshApprenticeshipForAccountMessage message,
            [Queue(QueueNames.StoreApprenticeships)] ICollector<ApprenticeshipMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<GetApprenticeshipsForEmployer>(writer, executionContext,
               async (container, logger) =>
               {
                   logger.Debug($"Getting apprenticeships for employer {message.EmployerId}...");
                   var employerCommitmentsApi = container.GetInstance<IEmployerCommitmentApi>();
                   var apprenticeshipValidation = new ApprenticeshipValidation();
                   var mapper = new Mapper(container.GetInstance<IApprenticeshipCourseDataService>());

                   var apprenticeships = await GetApprenticeshipsForAccount(message.EmployerId, employerCommitmentsApi);

                   IEnumerable<long> failedValidation;

                   (apprenticeships, failedValidation) = apprenticeshipValidation.BusinessValidation(apprenticeships);
                   logger.Info($"{failedValidation.Count()} apprenticeships failed business validation");

                   (apprenticeships, failedValidation)  = apprenticeshipValidation.InputValidation(apprenticeships);
                   logger.Info($"{failedValidation.Count()} apprenticeships failed input validation");

                   var mappedApprenticeships = apprenticeships.Select(mapper.Map).ToList();

                   logger.Info($"Sending {mappedApprenticeships.Count} apprenticeships for storing. EmployerId: {message.EmployerId} ");

                   foreach (var apprenticeship in mappedApprenticeships)
                   {
                       outputQueueMessage.Add(await apprenticeship);
                   }
               });
        }

        private static async Task<List<DAS.Commitments.Api.Types.Apprenticeship.Apprenticeship>> GetApprenticeshipsForAccount(long employerAccountId, IEmployerCommitmentApi employerCommitmentsApi)
        {
            var apiApprenticeships =
                await employerCommitmentsApi.GetEmployerApprenticeships(
                    employerAccountId, new ApprenticeshipSearchQuery
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
                            employerAccountId, new ApprenticeshipSearchQuery
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

            return apprenticeships;
        }
    }
}
