﻿using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.Forecasting.Application.Payments.Mapping;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Domain.Payments;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Forecasting.Application.Payments.Handlers
{
    public class ProcessEmployerPaymentHandler
    {
        public IEmployerPaymentRepository Repository { get; }
        public ILog Logger { get; }
        public IApplicationConfiguration ApplicationConfiguration { get; }

        public ProcessEmployerPaymentHandler(IEmployerPaymentRepository repository, ILog logger)
        {
            Repository = repository ?? throw new ArgumentNullException(nameof(repository));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Handle(PaymentCreatedMessage paymentCreatedMessage)
        {
	        var mapper = new PaymentMapper();
	        var employerPayment = mapper.MapToPayment(paymentCreatedMessage);

			Logger.Debug($"Now storing the employer payment. Employer: {employerPayment.EmployerAccountId}, year: {employerPayment.CollectionPeriod.Year}, month: {employerPayment.CollectionPeriod.Month}, Payment: {employerPayment.ToJson()}");
			await Repository.StorePayment(employerPayment);
            Logger.Info($"Finished adding the employer payment. Employer payment: {JsonConvert.SerializeObject(employerPayment)}");
        }
    }
}