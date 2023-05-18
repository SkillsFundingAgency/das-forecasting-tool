using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Data.Configurations
{
    public class AccountProjectionCommitmentConfiguration : IEntityTypeConfiguration<AccountProjectionCommitment>
    {
        public void Configure(EntityTypeBuilder<AccountProjectionCommitment> builder)
        {
            builder.ToTable("AccountProjectionCommitment");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.AccountProjectionId).HasColumnName(@"AccountProjectionId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.CommitmentId).HasColumnName(@"CommitmentId").HasColumnType("bigint").IsRequired();
        }
    }
}