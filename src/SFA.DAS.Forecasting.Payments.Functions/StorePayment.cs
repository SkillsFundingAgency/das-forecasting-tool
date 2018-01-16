using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.Forecasting.Payments.Application.Infrastructure;
using SFA.DAS.Forecasting.Payments.Domain.Apprenticeships;
using SFA.DAS.Forecasting.Payments.Domain.Entities;
using SFA.DAS.Forecasting.Payments.Messages.Events;

namespace SFA.DAS.Forecasting.Payments.Functions
{
    public class StorePayment: IFunction
    {
        [FunctionName("StorePayment")]
        public static async Task Run([QueueTrigger(QueueNames.StorePaymentEvent)]PaymentEvent payment, TraceWriter log)
        {
            await FunctionRunner.Run<StorePayment>(log, async container =>
            {
                var service = container.GetInstance<IApprenticeshipService>();
                await service.AddPayment(payment);
            });

            log.Info($"Finished handling payment event");

            ////var connectionString = Environment.GetEnvironmentVariable("StorageConnectionString", EnvironmentVariableTarget.Process);
            ////var paymentRepository = new PaymentsRepository(connectionString);
            ////await paymentRepository.StorePayment(payment);

            //var apprenticeship =  Map(payment);

            //var connectionString = Environment.GetEnvironmentVariable("DbConnectionString", EnvironmentVariableTarget.Process);

            //var paymentsdb = new ApprenticeshipRepository(connectionString, new SFA.DAS.NLog.Logger.NLogLogger());
            //await paymentsdb.InsertOrUpdatePayment(apprenticeship);

        }

       
    }
}
