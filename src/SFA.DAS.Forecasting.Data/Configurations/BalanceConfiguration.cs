using SFA.DAS.Forecasting.Models.Balance;

namespace SFA.DAS.Forecasting.Data.Configurations
{
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class BalanceConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<BalanceModel>
    {
        public BalanceConfiguration()
            : this("dbo")
        {
        }

        public BalanceConfiguration(string schema)
        {
            ToTable("Balance", schema);
            HasKey(x => x.EmployerAccountId);

            Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").HasColumnType("bigint").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Amount).HasColumnName(@"Amount").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            Property(x => x.TransferAllowance).HasColumnName(@"TransferAllowance").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            Property(x => x.RemainingTransferBalance).HasColumnName(@"RemainingTransferBalance").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            Property(x => x.BalancePeriod).HasColumnName(@"BalancePeriod").HasColumnType("datetime").IsRequired();
            Property(x => x.ReceivedDate).HasColumnName(@"ReceivedDate").HasColumnType("datetime").IsRequired();
            InitializePartial();
        }
        partial void InitializePartial();
    }
}