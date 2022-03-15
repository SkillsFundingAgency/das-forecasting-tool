using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.CommitmentsV2.Api.Client;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Commitments.Functions.Application;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class GetApprenticeshipsForEmployer : IFunction
    {
        public const int PageItemCount = 500;

        [FunctionName("GetApprenticeshipsForEmployer")]
        public static async Task Run(
            [QueueTrigger(QueueNames.RefreshApprenticeshipsForEmployer)] RefreshApprenticeshipForAccountMessage message,
            [Queue(QueueNames.StoreApprenticeships)] ICollector<ApprenticeshipMessage> outputQueueMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {
            await FunctionRunner.Run<GetApprenticeshipsForEmployer>(writer, executionContext,
               async (container, logger) =>
               {
                   logger.Info("Getting pledges and applications");
                   try
                   {
                       var pledgesService = container.GetInstance<IPledgesService>();
                       await pledgesService.GetPledges();
                   }
                   catch (Exception ex)
                   {
                       logger.Error(ex, "Exception getting pledges");
                       throw;
                   }


                   logger.Info($"Getting apprenticeships for employer {message.EmployerId}...");
                   var employerCommitmentsApi = container.GetInstance<ICommitmentsApiClient>();
                   var apprenticeshipValidation = new ApprenticeshipValidation();
                   var mapper = new Mapper(container.GetInstance<IApprenticeshipCourseDataService>());

                   var apprenticeships = await GetApprenticeshipsForAccount(message.EmployerId, employerCommitmentsApi);

                   IEnumerable<long> failedValidation;

                   (apprenticeships, failedValidation) = apprenticeshipValidation.BusinessValidation(apprenticeships);
                   logger.Info($"{failedValidation.Count()} apprenticeships failed business validation");

                   var mappedApprenticeships = apprenticeships.Select(y => mapper.Map(y, message.EmployerId)).ToList();

                   logger.Info($"Sending {mappedApprenticeships.Count} apprenticeships for storing. EmployerId: {message.EmployerId} ");

                   foreach (var apprenticeship in mappedApprenticeships)
                   {
                       outputQueueMessage.Add(await apprenticeship);
                   }
               });
        }

        private static async Task<List<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>> GetApprenticeshipsForAccount(long employerAccountId, ICommitmentsApiClient employerCommitmentsApi)
        {
            var liveApprenticeshipsTask = GetApprenticeshipsForStatus(employerAccountId, employerCommitmentsApi, CommitmentsV2.Types.ApprenticeshipStatus.Live);
            var awaitingApprenticeshipsTask = GetApprenticeshipsForStatus(employerAccountId, employerCommitmentsApi, CommitmentsV2.Types.ApprenticeshipStatus.WaitingToStart);

            var apprenticeships = await liveApprenticeshipsTask;
            apprenticeships.AddRange(await awaitingApprenticeshipsTask);

            return apprenticeships;
        }

        private static async Task<List<GetApprenticeshipsResponse.ApprenticeshipDetailsResponse>> GetApprenticeshipsForStatus(long employerAccountId, ICommitmentsApiClient employerCommitmentsApi, CommitmentsV2.Types.ApprenticeshipStatus status)
        {
            GetApprenticeshipsResponse apiApprenticeships = await GetApprenticeships(employerCommitmentsApi, employerAccountId, status);

            var pageNumber = 2;

            var totalPages = (int)Math.Ceiling((double)apiApprenticeships.TotalApprenticeshipsFound / PageItemCount);

            var apprenticeships = apiApprenticeships.Apprenticeships.ToList();

            if (totalPages > 1)
            {
                while (pageNumber <= totalPages)
                {
                    var pageOfApprenticeshipSearchResponse =
                       await GetApprenticeships(employerCommitmentsApi, employerAccountId, status, pageNumber);
                    apprenticeships.AddRange(pageOfApprenticeshipSearchResponse.Apprenticeships);

                    pageNumber++;
                }
            }

            return apprenticeships;
        }

        private static async Task<GetApprenticeshipsResponse> GetApprenticeships(ICommitmentsApiClient employerCommitmentsApi, long employerAccountId, CommitmentsV2.Types.ApprenticeshipStatus status, int? pageNumber = null)
        {
            var request = new CommitmentsV2.Api.Types.Requests.GetApprenticeshipsRequest
            {
                Status = status,
                AccountId = employerAccountId,
                PageNumber = pageNumber ?? 1,
                PageItemCount = PageItemCount
            };

            return await employerCommitmentsApi.GetApprenticeships(request);
        }
    }
}
