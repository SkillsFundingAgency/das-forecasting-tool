using SFA.DAS.Forecasting.Models.Levy;

namespace SFA.DAS.Forecasting.Data.Configurations
{
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class LevyDeclarationConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<LevyDeclarationModel>
    {
        public LevyDeclarationConfiguration()
            : this("dbo")
        {
        }

        public LevyDeclarationConfiguration(string schema)
        {
            ToTable("LevyDeclaration", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(x => x.SubmissionId).HasColumnName(@"SubmissionId").HasColumnType("bigint").IsRequired();
            Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").HasColumnType("bigint").IsRequired();
            Property(x => x.Scheme).HasColumnName(@"Scheme").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            Property(x => x.PayrollYear).HasColumnName(@"PayrollYear").HasColumnType("nvarchar").IsRequired().HasMaxLength(10);
            Property(x => x.PayrollMonth).HasColumnName(@"PayrollMonth").HasColumnType("tinyint").IsRequired();
            Property(x => x.PayrollDate).HasColumnName(@"PayrollDate").HasColumnType("datetime").IsRequired();
            Property(x => x.LevyAmountDeclared).HasColumnName(@"LevyAmountDeclared").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            Property(x => x.TransactionDate).HasColumnName(@"TransactionDate").HasColumnType("datetime").IsRequired();
            Property(x => x.DateReceived).HasColumnName(@"DateReceived").HasColumnType("datetime").IsRequired();
            InitializePartial();
        }
        partial void InitializePartial();
    }
}