using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Data.Configurations
{
    public class AccountProjectionConfiguration : IEntityTypeConfiguration<AccountProjectionModel>
    {
        public void Configure(EntityTypeBuilder<AccountProjectionModel> builder)
        {
            builder.ToTable("AccountProjection");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.ProjectionCreationDate).HasColumnName(@"ProjectionCreationDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ProjectionGenerationType).HasColumnName(@"ProjectionGenerationType").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.Month).HasColumnName(@"Month").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.Year).HasColumnName(@"Year").HasColumnType("int").IsRequired();

            builder.Property(x => x.LevyFundsIn).HasColumnName(@"FundsIn").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            builder.Property(x => x.LevyFundedCostOfTraining).HasColumnName(@"TotalCostOfTraining").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            builder.Property(x => x.LevyFundedCompletionPayments).HasColumnName(@"CompletionPayments").HasColumnType("decimal").IsRequired().HasPrecision(18,5);

            builder.Property(x => x.TransferInCostOfTraining).HasColumnName(@"TransferInTotalCostOfTraining").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);
            builder.Property(x => x.TransferOutCostOfTraining).HasColumnName(@"TransferOutTotalCostOfTraining").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);

            builder.Property(x => x.TransferInCompletionPayments).HasColumnName(@"TransferInCompletionPayments").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);
            builder.Property(x => x.TransferOutCompletionPayments).HasColumnName(@"TransferOutCompletionPayments").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);

            builder.Property(x => x.ApprovedPledgeApplicationCost).HasColumnName(@"ApprovedPledgeApplicationCost").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);
            builder.Property(x => x.AcceptedPledgeApplicationCost).HasColumnName(@"AcceptedPledgeApplicationCost").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);
            builder.Property(x => x.PledgeOriginatedCommitmentCost).HasColumnName(@"PledgeOriginatedCommitmentCost").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);

            builder.Property(x => x.FutureFunds).HasColumnName(@"FutureFunds").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            builder.Property(x => x.CoInvestmentEmployer).HasColumnName(@"CoInvestmentEmployer").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            builder.Property(x => x.CoInvestmentGovernment).HasColumnName(@"CoInvestmentGovernment").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            builder.Property(x => x.ExpiredFunds).HasColumnName(@"ExpiredFunds").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);
            builder.Property(x => x.FutureFundsNoExpiry).HasColumnName(@"FutureFundsNoExpiry").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);
            builder.Ignore(x => x.CommittedTransferCost);
            builder.Ignore(x => x.CommittedTransferCompletionCost);
            builder.Ignore(x => x.IsFirstMonth);
        }
    }
}