using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Data.Configurations
{
    public class CommitmentConfiguration : IEntityTypeConfiguration<CommitmentModel>
    {
        public void Configure(EntityTypeBuilder<CommitmentModel> builder)
        {
            builder.ToTable("Commitment");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.LearnerId).HasColumnName(@"LearnerId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.ProviderId).HasColumnName(@"ProviderId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.ProviderName).HasColumnName(@"ProviderName").HasColumnType("nvarchar").IsRequired().HasMaxLength(200);
            builder.Property(x => x.ApprenticeshipId).HasColumnName(@"ApprenticeshipId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.ApprenticeName).HasColumnName(@"ApprenticeName").HasColumnType("nvarchar").IsRequired().HasMaxLength(200);
            builder.Property(x => x.CourseName).HasColumnName(@"CourseName").HasColumnType("nvarchar").IsRequired().HasMaxLength(200);
            builder.Property(x => x.CourseLevel).HasColumnName(@"CourseLevel").HasColumnType("int").IsRequired(false);
            builder.Property(x => x.StartDate).HasColumnName(@"StartDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.PlannedEndDate).HasColumnName(@"PlannedEndDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ActualEndDate).HasColumnName(@"ActualEndDate").HasColumnType("datetime").IsRequired(false);
            builder.Property(x => x.CompletionAmount).HasColumnName(@"CompletionAmount").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            builder.Property(x => x.MonthlyInstallment).HasColumnName(@"MonthlyInstallment").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            builder.Property(x => x.NumberOfInstallments).HasColumnName(@"NumberOfInstallments").HasColumnType("smallint").IsRequired();
            builder.Property(x => x.HasHadPayment).HasColumnName(@"HasHadPayment").HasColumnType("bit").IsRequired();
            builder.Property(x => x.UpdatedDateTime).HasColumnName(@"UpdatedDateTime").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.FundingSource).HasColumnName(@"FundingSource").HasColumnType("tinyint").IsRequired();
        }
    }
}