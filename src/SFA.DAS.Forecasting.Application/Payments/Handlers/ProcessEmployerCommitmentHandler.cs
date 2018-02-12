using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public class ProcessEmployerCommitmentHandler
    {
        public IEmployerPaymentRepository Repository { get; }
        public ILog Logger { get; }
        public IApplicationConfiguration ApplicationConfiguration { get; }

        public ProcessEmployerCommitmentHandler(IEmployerPaymentRepository repository, ILog logger)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(Payment employerPayment)
        {
            Logger.Debug($"Now storing the employer payment. Employer: {employerPayment.EmployerAccountId}, year: {employerPayment.CollectionPeriod.Year}, month: {employerPayment.CollectionPeriod.Month}");
			await Repository.StorePayment(employerPayment);
            Logger.Info($"Finished adding the employer payment. Employer payment: {JsonConvert.SerializeObject(employerPayment)}");
        }
    }
}