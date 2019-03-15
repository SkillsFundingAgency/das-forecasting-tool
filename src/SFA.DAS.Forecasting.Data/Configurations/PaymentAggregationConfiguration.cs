using SFA.DAS.Forecasting.Models.Payments;

namespace SFA.DAS.Forecasting.Data.Configurations
{
    public partial class PaymentAggregationConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<PaymentAggregationModel>
    {
        public PaymentAggregationConfiguration() :this("dbo")
        {
            
        }

        public PaymentAggregationConfiguration(string schema)
        {
            ToTable("PaymentAggregation", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").HasColumnType("bigint").IsRequired();
            Property(x => x.CollectionPeriod.Month).HasColumnName(@"CollectionPeriodMonth").HasColumnType("int").IsRequired();
            Property(x => x.CollectionPeriod.Year).HasColumnName(@"CollectionPeriodYear").HasColumnType("int").IsRequired();
            Property(x => x.Amount).HasColumnName(@"Amount").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);

            InitializePartial();
        }

        partial void InitializePartial();
    }
}
