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

            Property(x => x.LevyFundsIn).HasColumnName(@"FundsIn").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            Property(x => x.LevyFundedCostOfTraining).HasColumnName(@"TotalCostOfTraining").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            Property(x => x.LevyFundedCompletionPayments).HasColumnName(@"CompletionPayments").HasColumnType("decimal").IsRequired().HasPrecision(18,5);

            Property(x => x.TransferInCostOfTraining).HasColumnName(@"TransferInTotalCostOfTraining").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);
            Property(x => x.TransferOutCostOfTraining).HasColumnName(@"TransferOutTotalCostOfTraining").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);

            Property(x => x.TransferInCompletionPayments).HasColumnName(@"TransferInCompletionPayments").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);
            Property(x => x.TransferOutCompletionPayments).HasColumnName(@"TransferOutCompletionPayments").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);

            Property(x => x.FutureFunds).HasColumnName(@"FutureFunds").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            Property(x => x.CoInvestmentEmployer).HasColumnName(@"CoInvestmentEmployer").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            Property(x => x.CoInvestmentGovernment).HasColumnName(@"CoInvestmentGovernment").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            Property(x => x.ExpiredFunds).HasColumnName(@"ExpiredFunds").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);
            Property(x => x.FutureFundsNoExpiry).HasColumnName(@"FutureFundsNoExpiry").HasColumnType("decimal").IsRequired().HasPrecision(18, 5);
            Ignore(x => x.CommittedTransferCost);
            Ignore(x => x.CommittedTransferCompletionCost);
            Ignore(x => x.IsFirstMonth);
            InitializePartial();
        }
        partial void InitializePartial();
    }
}