using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Forecasting.Models.Balance;

namespace SFA.DAS.Forecasting.Data.Configurations
{
    public class BalanceConfiguration : IEntityTypeConfiguration<BalanceModel>
    {
        public void Configure(EntityTypeBuilder<BalanceModel> builder)
        {
            builder.ToTable("Balance");
            builder.HasKey(x => x.EmployerAccountId);

            builder.Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").HasColumnType("bigint").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.Amount).HasColumnName(@"Amount").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            builder.Property(x => x.TransferAllowance).HasColumnName(@"TransferAllowance").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            builder.Property(x => x.RemainingTransferBalance).HasColumnName(@"RemainingTransferBalance").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            builder.Property(x => x.BalancePeriod).HasColumnName(@"BalancePeriod").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.ReceivedDate).HasColumnName(@"ReceivedDate").HasColumnType("datetime").IsRequired();
        }
    }
}