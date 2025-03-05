using System;
using SFA.DAS.Forecasting.Application.Apprenticeship.Messages;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Application.Apprenticeship.Mapping;

public static class ApprenticeshipMapping
{
    public static CommitmentModel MapToCommitment(ApprenticeshipMessage message)
    {
        var model = new CommitmentModel
        {
            EmployerAccountId = message.EmployerAccountId,
            ApprenticeshipId = message.ApprenticeshipId,
            LearnerId = message.LearnerId,
            StartDate = message.StartDate,
            PlannedEndDate = message.PlannedEndDate,
            ActualEndDate = message.ActualEndDate,
            CompletionAmount = message.CompletionAmount,
            MonthlyInstallment = message.MonthlyInstallment,
            NumberOfInstallments = (short)message.NumberOfInstallments,
            ProviderId = message.ProviderId,
            ProviderName = message.ProviderName,
            ApprenticeName = message.ApprenticeName,
            CourseName = message.CourseName,
            CourseLevel = message.CourseLevel,
            SendingEmployerAccountId = message.SendingEmployerAccountId ?? message.EmployerAccountId,
            FundingSource = message.FundingSource,
            UpdatedDateTime = DateTime.UtcNow,
            HasHadPayment = false,
            PledgeApplicationId = message.PledgeApplicationId
        };

        return model;
    }
}