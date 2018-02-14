using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public class ProcessEmployerCommitmentHandler
    {
        public ICommitmentRepository Repository { get; }
        public ILog Logger { get; }
        public IApplicationConfiguration ApplicationConfiguration { get; }

        public ProcessEmployerCommitmentHandler(ICommitmentRepository repository, ILog logger)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PaymentCreatedMessage paymentCreatedMessage)
        {
			var mapper = new PaymentMapper();
			var employerCommitment = mapper.MapToCommitment(paymentCreatedMessage);

			Logger.Debug($"Now storing the employer commitment. Employer: {employerCommitment.EmployerAccountId}, ApprenticeshipId: {employerCommitment.ApprenticeshipId}");
			await Repository.StoreCommitment(employerCommitment);
            Logger.Info($"Finished adding the employer commitment. Employer commitment: {JsonConvert.SerializeObject(employerCommitment)}");
        }
    }
}