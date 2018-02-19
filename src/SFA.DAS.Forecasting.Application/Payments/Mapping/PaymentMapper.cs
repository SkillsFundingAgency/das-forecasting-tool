using System;
using SFA.DAS.Forecasting.Application.Payments.Messages;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Application.Payments.Mapping
{
	public class PaymentMapper
	{
		public Payment MapToPayment(PaymentCreatedMessage paymentCreatedMessage)
		{
			return new Payment
			{
				ExternalPaymentId = paymentCreatedMessage.Id,
				EmployerAccountId = paymentCreatedMessage.EmployerAccountId,
				ProviderId = paymentCreatedMessage.Ukprn,
				LearnerId = paymentCreatedMessage.Uln,
				Amount = paymentCreatedMessage.Amount,
				CollectionPeriod = new Models.Payments.CollectionPeriod
                {
					Id = paymentCreatedMessage.CollectionPeriod.Id,
					Month = paymentCreatedMessage.CollectionPeriod.Month,
					Year = paymentCreatedMessage.CollectionPeriod.Year
				},
				ApprenticeshipId = paymentCreatedMessage.ApprenticeshipId,
				ReceivedTime = DateTime.Now,
				FundingSource = FundingSource.Levy
			};
		}

		public Commitment MapToCommitment(PaymentCreatedMessage paymentCreatedMessage)
		{
			return new Commitment
			{
				EmployerAccountId = paymentCreatedMessage.EmployerAccountId,
				ApprenticeshipId = paymentCreatedMessage.ApprenticeshipId,
				LearnerId = paymentCreatedMessage.Uln,
				StartDate = paymentCreatedMessage.EarningDetails.StartDate,
				PlannedEndDate = paymentCreatedMessage.EarningDetails.PlannedEndDate,
				ActualEndDate = paymentCreatedMessage.EarningDetails.ActualEndDate,
				CompletionAmount = paymentCreatedMessage.EarningDetails.CompletionAmount,
				MonthlyInstallment = paymentCreatedMessage.EarningDetails.MonthlyInstallment,
				NumberOfInstallments = (short)paymentCreatedMessage.EarningDetails.TotalInstallments,
				ProviderId = paymentCreatedMessage.Ukprn,
				ProviderName = paymentCreatedMessage.ProviderName,
				ApprenticeName = paymentCreatedMessage.ApprenticeName,
				CourseName = paymentCreatedMessage.CourseName,
				CourseLevel = paymentCreatedMessage.CourseLevel
			};
		}
	}
}
