using System.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using NUnit.Framework;
using SFA.DAS.Forecasting.PerformanceTests.Infrastructure;

namespace SFA.DAS.Forecasting.PerformanceTests.Projections
{
    [TestFixture]
    public class ProjectionsTests
    {
        private FunctionFunner functionFunner;
        private CloudStorageAccount account;
        private CloudQueueClient cloudQueueClient;

        [SetUp]
        public void SetUp()
        {
            account = CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageConnectionString"]?.ConnectionString);
            cloudQueueClient = account.CreateCloudQueueClient();
            ClearQueues();

            functionFunner = new FunctionFunner();
            functionFunner.StartFunction("SFA.DAS.Forecasting.StubApi.Functions");
            functionFunner.StartFunction("SFA.DAS.Forecasting.Projections.Functions");
        }

        private void ClearQueues()
        {
            cloudQueueClient.ClearQueue(QueueNames.Projections.BuildProjections);
            cloudQueueClient.ClearQueue(QueueNames.Projections.GenerateProjections);
            cloudQueueClient.ClearQueue(QueueNames.Projections.GetAccountBalance);
        }

        [TearDown]
        public void CleanUp()
        {
            functionFunner?.StopFunctions();
        }

        

        //protected void InsertCommitments(List<TestCommitment> commitments)
        //{
        //    var senderId = CommitmentType == CommitmentType.TransferReceiver ? 54321 : EmployerAccountId;
        //    var receiverId = CommitmentType == CommitmentType.TransferSender ? 54321 : EmployerAccountId;

        //    for (var i = 0; i < commitments.Count; i++)
        //    {
        //        var commitment = commitments[i];

        //        var isTransferSender = CommitmentType == CommitmentType.TransferSender;
        //        var isFundingSourceLevy = commitment.FundingSource.HasValue && commitment.FundingSource == FundingSource.Levy;

        //        DataContext.Commitments.Add(new CommitmentModel
        //        {
        //            EmployerAccountId = isTransferSender && isFundingSourceLevy ? EmployerAccountId : receiverId,
        //            LearnerId = i + 1,
        //            ApprenticeshipId = commitment.ApprenticeshipId > 0 ? commitment.ApprenticeshipId : i + 2,
        //            ApprenticeName = commitment.ApprenticeName,
        //            SendingEmployerAccountId = senderId,
        //            ProviderId = i + 3,
        //            ProviderName = commitment.ProviderName,
        //            CourseName = commitment.CourseName,
        //            CourseLevel = commitment.CourseLevel,
        //            StartDate = commitment.StartDateValue,
        //            PlannedEndDate = commitment.PlannedEndDate,
        //            ActualEndDate = commitment.ActualEndDateValue.Value == DateTime.MinValue ? null : commitment.ActualEndDateValue,
        //            CompletionAmount = commitment.CompletionAmount,
        //            MonthlyInstallment = commitment.InstallmentAmount,
        //            NumberOfInstallments = (short)commitment.NumberOfInstallments,
        //            FundingSource = CommitmentType == CommitmentType.LevyFunded
        //                ? FundingSource.Levy
        //                : commitment.FundingSource ?? FundingSource.Transfer
        //        });
        //    }

        //    DataContext.SaveChanges();
        //}
    }
}