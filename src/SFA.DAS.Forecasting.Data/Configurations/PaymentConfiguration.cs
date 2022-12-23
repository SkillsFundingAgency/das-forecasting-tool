using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Data.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<PaymentModel>
    {
        public void Configure(EntityTypeBuilder<PaymentModel> builder)
        {
            builder.ToTable("Payment");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.ExternalPaymentId).HasColumnName(@"ExternalPaymentId").HasColumnType("nvarchar").IsRequired().HasMaxLength(100);
            builder.Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.ProviderId).HasColumnName(@"ProviderId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.ApprenticeshipId).HasColumnName(@"ApprenticeshipId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.Amount).HasColumnName(@"Amount").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            builder.Property(x => x.ReceivedTime).HasColumnName(@"ReceivedTime").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.LearnerId).HasColumnName(@"LearnerId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.CollectionPeriod.Month).HasColumnName(@"CollectionPeriodMonth").HasColumnType("int").IsRequired();
            builder.Property(x => x.CollectionPeriod.Year).HasColumnName(@"CollectionPeriodYear").HasColumnType("int").IsRequired();
            builder.Property(x => x.DeliveryPeriod.Month).HasColumnName(@"DeliveryPeriodMonth").HasColumnType("int").IsRequired();
            builder.Property(x => x.DeliveryPeriod.Year).HasColumnName(@"DeliveryPeriodYear").HasColumnType("int").IsRequired();
            builder.Property(x => x.FundingSource).HasColumnName(@"FundingSource").HasColumnType("tinyint").IsRequired();
        }
    }
}