using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Payments.Application.Infrastructure;
using SFA.DAS.Forecasting.Payments.Domain.Entities;
using SFA.DAS.Forecasting.Payments.Messages.Events;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public static class StorePayment
    {
        [FunctionName("StorePayment")]
        public static async Task Run([QueueTrigger(QueueNames.StorePaymentEvent)]PaymentEvent payment, TraceWriter log)
        {
            log.Info($"Storing payment to database");

            //var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString", EnvironmentVariableTarget.Process);
            //var paymentRepository = new PaymentsRepository(connectionString);
            //await paymentRepository.StorePayment(payment);

            var apprenticeship =  Map(payment);

            var connectionString = Environment.GetEnvironmentVariable("DbConnectionString", EnvironmentVariableTarget.Process);

            var paymentsdb = new PaymentDbRepository(connectionString, new SFA.DAS.NLog.Logger.NLogLogger());
            await paymentsdb.InsertOrUpdatePayment(apprenticeship);

        }

        private static PaymentApprenticeship Map(PaymentEvent payment)
        {
            return new PaymentApprenticeship
            {
                EmployerAccountId = long.Parse(payment.EmployerAccountId),
                Name = "Gustav",
                Uln = payment.Uln,
                DateOfBirth = new DateTime(1998, 12, 08),
                TrainingName = "Code manager",
                TrainingLevel = 2,
                TrainingProviderName = "Provider name",
                StartDate = payment.EarningDetails.StartDate,
                MonthlyPayment = payment.EarningDetails.MonthlyInstallment,
                Instalments = payment.EarningDetails.TotalInstallments,
                CompletionPayment = payment.EarningDetails.CompletionAmount
            };
        }
    }
}
