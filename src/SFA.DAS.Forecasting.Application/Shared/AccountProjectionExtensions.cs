using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SFA.DAS.Forecasting.Domain.Projections;
using SFA.DAS.Forecasting.Messages.Projections;
using SFA.DAS.Forecasting.Models.Projections;
using AccountProjectionModel = SFA.DAS.Forecasting.Messages.Projections.AccountProjectionModel;

namespace SFA.DAS.Forecasting.Application.Shared
{
    public static class AccountProjectionExtensions
    {
        public static AccountProjectionCreatedEvent MapToEvent(this AccountProjection input)
        {
            var projectionEvent = new AccountProjectionCreatedEvent(input.EmployerAccountId)
            {
                ProjectionModels = input.Projections.Select(s =>
                    new AccountProjectionModel()
                    {
                        EmployerAccountId = s.EmployerAccountId,
                        ProjectionCreationDate = s.ProjectionCreationDate, // ProjectionCreationDate
                        ProjectionGenerationType =
                            ConvertProjectionType(s.ProjectionGenerationType), // ProjectionGenerationType
                        Month = s.Month, // Month
                        Year = s.Year, // Year
                        LevyFundsIn = s.LevyFundsIn,
                        LevyFundedCostOfTraining = s.LevyFundedCostOfTraining,
                        LevyFundedCompletionPayments = s.LevyFundedCompletionPayments,
                        TransferInCostOfTraining = s.TransferInCostOfTraining,
                        TransferOutCostOfTraining = s.TransferInCostOfTraining,
                        TransferInCompletionPayments = s.TransferInCompletionPayments,
                        TransferOutCompletionPayments = s.TransferOutCompletionPayments,
                        CommittedTransferCost = s.CommittedTransferCost,
                        CommittedTransferCompletionCost = s.CommittedTransferCompletionCost,
                        //ExpiredFunds = s.ExpiredFunds,
                        FutureFunds = s.FutureFunds, // FutureFunds
                        CoInvestmentEmployer = s.CoInvestmentEmployer, // CoInvestmentEmployer
                        CoInvestmentGovernment = s.CoInvestmentGovernment // CoInvestmentGovernment
                    })
            };


            return projectionEvent;
        }

        private static ProjectionSource ConvertProjectionType(ProjectionGenerationType type)
        {
            switch (type)
            {
                case ProjectionGenerationType.PayrollPeriodEnd:
                    return ProjectionSource.PaymentPeriodEnd;
                    break;
                case ProjectionGenerationType.LevyDeclaration:
                    return ProjectionSource.LevyDeclaration;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }
}