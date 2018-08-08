using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Functions.Framework;
using SFA.DAS.Forecasting.Models.Payments;

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
                   _apprenticeshipCourseDataService = container.GetInstance<IApprenticeshipCourseDataService>();

                   IEnumerable<ApiApprenticeship> apiApprenticeships = 
                      await employerCommitmentsApi.GetEmployerApprenticeships(message.EmployerId)
                      ?? new List<ApiApprenticeship>();

                   logger.Info($"Found {apiApprenticeships.Count()} apprenticeships in Commitments API for Employer: {message.EmployerId} ");

                   apiApprenticeships = BusinessValidation(apiApprenticeships);
                   apiApprenticeships = InputValidation(apiApprenticeships);

                   var apprenticeships = apiApprenticeships.Select(Map);

                   logger.Info($"Sending {apprenticeships.Count()} apprenticeships for storing. EmployerId: {message.EmployerId} ");

                   foreach (var apprenticeship in apprenticeships)
                   {
                       outputQueueMessage.Add(await apprenticeship);
                   }

               });
        }

        private static IEnumerable<ApiApprenticeship> BusinessValidation(IEnumerable<ApiApprenticeship> apprenticeships)
        {
            return apprenticeships
                .Where(m => !m.HasHadDataLockSuccess)
                .Where(m => m.StopDate == null)
                .Where(m => m.PauseDate == null)
                .Where(m => m.PaymentStatus == Commitments.Api.Types.Apprenticeship.Types.PaymentStatus.Active);
        }

        private static IEnumerable<ApiApprenticeship> InputValidation(IEnumerable<ApiApprenticeship> apprenticeships)
        {
            return apprenticeships
                .Where(m => m.StartDate.HasValue)
                .Where(m => m.EndDate.HasValue)
                .Where(m => m.Cost.HasValue);
        }

        private static async Task<ApprenticeshipMessage> Map(ApiApprenticeship apprenticeship)
        {
            int NumberOfInstallments(DateTime start, DateTime end)
            {
                var count = 0;
                while (start < end) { count++; start = end.AddMonths(1); }
                return count;
            }
            var duration = NumberOfInstallments(apprenticeship.StartDate.Value, apprenticeship.EndDate.Value);
            var training = await _apprenticeshipCourseDataService.GetApprenticeshipCourse(apprenticeship.TrainingCode);

            return new ApprenticeshipMessage
            {
                EmployerAccountId = apprenticeship.EmployerAccountId,
                SendingEmployerAccountId = apprenticeship.TransferSenderId,
                LearnerId = long.TryParse(apprenticeship.ULN, out long result) ? result : 0,
                ProviderId = apprenticeship.ProviderId,
                ProviderName = apprenticeship.ProviderName,
                ApprenticeshipId = apprenticeship.Id,
                ApprenticeName = $"{apprenticeship.FirstName} {apprenticeship.LastName}",
                CourseName = apprenticeship.TrainingName,
                CourseLevel = training?.Level ?? 0,
                StartDate = apprenticeship.StartDate.Value,
                PlannedEndDate = apprenticeship.EndDate.Value,
                ActualEndDate = null,
                CompletionAmount = apprenticeship.Cost.Value * 0.8M,
                MonthlyInstallment = (apprenticeship.Cost.Value * 0.2M) / duration,
                NumberOfInstallments = duration,
                FundingSource = apprenticeship.TransferSenderId == null ? FundingSource.Levy : FundingSource.Transfer
            };
        }
    }
}
