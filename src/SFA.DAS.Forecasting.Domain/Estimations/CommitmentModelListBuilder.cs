using System.Collections.Generic;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Estimation;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Domain.Estimations
{
    public interface ICommitmentModelListBuilder
    {
        List<CommitmentModel> Build(long accountId, IReadOnlyCollection<VirtualApprenticeship> modelledApprenticeships);
    }

    public class CommitmentModelListBuilder: ICommitmentModelListBuilder
    {
        public List<CommitmentModel> Build(long accountId, IReadOnlyCollection<VirtualApprenticeship> modelledApprenticeships)
        {
            var commitments = new List<CommitmentModel>();
            foreach (var virtualApprenticeships in modelledApprenticeships)
            {
                for (var i = 0; i < virtualApprenticeships.ApprenticesCount; i++)
                {
                    commitments.Add(new CommitmentModel
                    {
                        CompletionAmount = virtualApprenticeships.TotalCompletionAmount / virtualApprenticeships.ApprenticesCount,
                        //EmployerAccountId = accountId,
                        SendingEmployerAccountId = accountId,
                        ActualEndDate = null,
                        MonthlyInstallment = virtualApprenticeships.TotalInstallmentAmount / virtualApprenticeships.ApprenticesCount,
                        NumberOfInstallments = virtualApprenticeships.TotalInstallments,
                        PlannedEndDate = virtualApprenticeships.StartDate.AddMonths(virtualApprenticeships.TotalInstallments),
                        StartDate = virtualApprenticeships.StartDate,
                        FundingSource = virtualApprenticeships.FundingSource != 0 ? virtualApprenticeships.FundingSource : FundingSource.Transfer
                    });
                }
            }

            return commitments;
        }
    }
}