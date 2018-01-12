using System;
using SFA.DAS.Forecasting.Payments.Application.Infrastructure;
using SFA.DAS.Forecasting.Payments.Domain.Entities;
using SFA.DAS.Forecasting.Payments.Domain.Interfaces;
using StructureMap;

namespace SFA.DAS.PaymentsAdapter.Functions.Infrastructure
{
    public class DefaultRegistry: Registry
    {
        public DefaultRegistry()
        {
            if (Environment.GetEnvironmentVariable("Environment", EnvironmentVariableTarget.Process)
                    ?.Equals("DEV", StringComparison.OrdinalIgnoreCase) ?? false)
                ForSingletonOf<IPaymentEvents>().Use<DevPaymentEvents>();
            else
            {
                ForSingletonOf<PaymentEventsApiConfig>().Use(PaymentsConfig());
                ForSingletonOf<IPaymentEvents>().Use<PaymentEvents>();
            }
        }

        public static PaymentEventsApiConfig PaymentsConfig()
        {
            var url = Environment.GetEnvironmentVariable("PaymentsApiBaseUrl", EnvironmentVariableTarget.Process);
            var token = Environment.GetEnvironmentVariable("PaymentsApiToken", EnvironmentVariableTarget.Process);
            var conectionString = Environment.GetEnvironmentVariable("StorageConnectionString", EnvironmentVariableTarget.Process);
            return new PaymentEventsApiConfig
            {
                ApiBaseUrl = url,
                ClientToken = token,
                StorageConnectionString = conectionString,
                MaxPages = 5
            };
        }
    }


}