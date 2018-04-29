using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Data.Configurations
{
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class AccountProjectionConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<AccountProjectionModel>
    {
        public AccountProjectionConfiguration()
            : this("dbo")
        {
        }

        public AccountProjectionConfiguration(string schema)
        {
            ToTable("AccountProjection", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").HasColumnType("bigint").IsRequired();
            Property(x => x.ProjectionCreationDate).HasColumnName(@"ProjectionCreationDate").HasColumnType("datetime").IsRequired();
            Property(x => x.ProjectionGenerationType).HasColumnName(@"ProjectionGenerationType").HasColumnType("smallint").IsRequired();
            Property(x => x.Month).HasColumnName(@"Month").HasColumnType("smallint").IsRequired();
            Property(x => x.Year).HasColumnName(@"Year").HasColumnType("int").IsRequired();
            Property(x => x.FundsIn).HasColumnName(@"FundsIn").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            Property(x => x.TotalCostOfTraining).HasColumnName(@"TotalCostOfTraining").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            Property(x => x.CompletionPayments).HasColumnName(@"CompletionPayments").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            Property(x => x.FutureFunds).HasColumnName(@"FutureFunds").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            Property(x => x.CoInvestmentEmployer).HasColumnName(@"CoInvestmentEmployer").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            Property(x => x.CoInvestmentGovernment).HasColumnName(@"CoInvestmentGovernment").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            InitializePartial();
        }
        partial void InitializePartial();
    }
}