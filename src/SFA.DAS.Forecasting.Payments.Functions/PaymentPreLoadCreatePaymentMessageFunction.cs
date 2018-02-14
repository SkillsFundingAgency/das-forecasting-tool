using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Application.Shared.Services;
using SFA.DAS.Forecasting.Functions.Framework;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class PaymentPreLoadCreatePaymentMessageFunction : IFunction
    {
        [FunctionName("PaymentPreLoadCreatePaymentMessageFunction")]
        public static async Task<PaymentCreatedMessage> Run(
            [QueueTrigger(QueueNames.PreLoadPayment)]PreLoadPaymentMessage message, 
            TraceWriter writer)
        {
            return await FunctionRunner.Run<PaymentPreLoadAddEarningDetailsFunction, PaymentCreatedMessage>(writer,
               async (container, logger) =>
               {
                   var employerData = container.GetInstance<IEmployerDatabaseService>();

                   var payments = await employerData.GetEmployerPayments(message.EmployerAccountId, message.PeriodYear, message.PeriodMonth);
                   var payment = payments.SingleOrDefault();
                   logger.Info($"Creating new {nameof(PaymentCreatedMessage)} for {payment.AccountId}");
                   return
                    new PaymentCreatedMessage
                    {
                        Id = payment.PaymentId.ToString(),
                        EmployerAccountId = payment.AccountId,
                        Ukprn = payment.Ukprn,
                        ApprenticeshipId = payment.ApprenticeshipId,
                        Amount = payment.Amount,
                        ProviderName = payment.ProviderName,
                        ApprenticeName = payment.ApprenticeName,
                        CourseName = payment.ApprenticeshipCourseName,
                        CourseLevel = payment.ApprenticeshipCourseLevel,
                        Uln = payment.Uln,
                        CourseStartDate = payment.ApprenticeshipCourseStartDate,
                        CollectionPeriod = new CollectionPeriod { Id = payment.CollectionPeriodId, Year = payment.CollectionPeriodYear, Month = payment.CollectionPeriodMonth}
                        };
                });
        }
    }
}
