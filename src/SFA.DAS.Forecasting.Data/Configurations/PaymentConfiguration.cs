using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Data.Configurations
{
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class PaymentConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<PaymentModel>
    {
        public PaymentConfiguration()
            : this("dbo")
        {
        }

        public PaymentConfiguration(string schema)
        {
            ToTable("Payment", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(x => x.ExternalPaymentId).HasColumnName(@"ExternalPaymentId").HasColumnType("nvarchar").IsRequired().HasMaxLength(100);
            Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").HasColumnType("bigint").IsRequired();
            Property(x => x.ProviderId).HasColumnName(@"ProviderId").HasColumnType("bigint").IsRequired();
            Property(x => x.ApprenticeshipId).HasColumnName(@"ApprenticeshipId").HasColumnType("bigint").IsRequired();
            Property(x => x.Amount).HasColumnName(@"Amount").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            Property(x => x.ReceivedTime).HasColumnName(@"ReceivedTime").HasColumnType("datetime").IsRequired();
            Property(x => x.LearnerId).HasColumnName(@"LearnerId").HasColumnType("bigint").IsRequired();
            Property(x => x.CollectionPeriod.Month).HasColumnName(@"CollectionPeriodMonth").HasColumnType("int").IsRequired();
            Property(x => x.CollectionPeriod.Year).HasColumnName(@"CollectionPeriodYear").HasColumnType("int").IsRequired();
            Property(x => x.DeliveryPeriod.Month).HasColumnName(@"DeliveryPeriodMonth").HasColumnType("int").IsRequired();
            Property(x => x.DeliveryPeriod.Year).HasColumnName(@"DeliveryPeriodYear").HasColumnType("int").IsRequired();
            Property(x => x.FundingSource).HasColumnName(@"FundingSource").HasColumnType("tinyint").IsRequired();

            InitializePartial();
        }
        partial void InitializePartial();
    }
}