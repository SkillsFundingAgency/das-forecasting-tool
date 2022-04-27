using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Application.ApprenticeshipCourses.Services;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Commitments.Functions
{
    public class RefreshEmployersForPledgeApplications : IFunction
    {
        [FunctionName("RefreshEmployersForPledgeApplications")]
        public static async Task Run(
            [QueueTrigger(QueueNames.RefreshEmployersForPledgeApplications)] string message,
            [Queue(QueueNames.RefreshPledgeApplicationsForEmployer)] ICollector<RefreshPledgeApplicationsForAccountMessage> updatePledgeApplicationsForAccountOutputMessage,
            ExecutionContext executionContext,
            TraceWriter writer)
        {

            await FunctionRunner.Run<RefreshEmployersForPledgeApplications, string>(writer, executionContext,
                async (container, logger) =>
                {
                    logger.Info("Getting all pledges account ids...");

                    var accountIds = new List<long>();

                    try
                    {
                        var pledgesService = container.GetInstance<IPledgesService>();
                        accountIds = await pledgesService.GetAccountIds();
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex, "Exception getting account ids");
                        throw;
                    }

                    var count = 0;
                    foreach (var id in accountIds)
                    {
                        count++;
                        updatePledgeApplicationsForAccountOutputMessage.Add(new RefreshPledgeApplicationsForAccountMessage
                        {
                            EmployerId = id
                        });
                    }
                    var msg = $"Added {count} employer id messages to the queue {nameof(QueueNames.RefreshEmployersForPledgeApplications)}.";
                    logger.Info(msg);
                    return msg;
                });
        }
    }
}
