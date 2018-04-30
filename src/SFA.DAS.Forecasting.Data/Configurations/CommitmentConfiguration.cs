using SFA.DAS.Forecasting.Models.Commitments;

namespace SFA.DAS.Forecasting.Data.Configurations
{
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class CommitmentConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<CommitmentModel>
    {
        public CommitmentConfiguration()
            : this("dbo")
        {
        }

        public CommitmentConfiguration(string schema)
        {
            ToTable("Commitment", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").HasColumnType("bigint").IsRequired();
            Property(x => x.LearnerId).HasColumnName(@"LearnerId").HasColumnType("bigint").IsRequired();
            Property(x => x.ProviderId).HasColumnName(@"ProviderId").HasColumnType("bigint").IsRequired();
            Property(x => x.ProviderName).HasColumnName(@"ProviderName").HasColumnType("nvarchar").IsRequired().HasMaxLength(200);
            Property(x => x.ApprenticeshipId).HasColumnName(@"ApprenticeshipId").HasColumnType("bigint").IsRequired();
            Property(x => x.ApprenticeName).HasColumnName(@"ApprenticeName").HasColumnType("nvarchar").IsRequired().HasMaxLength(200);
            Property(x => x.CourseName).HasColumnName(@"CourseName").HasColumnType("nvarchar").IsRequired().HasMaxLength(200);
            Property(x => x.CourseLevel).HasColumnName(@"CourseLevel").HasColumnType("int").IsOptional();
            Property(x => x.StartDate).HasColumnName(@"StartDate").HasColumnType("datetime").IsRequired();
            Property(x => x.PlannedEndDate).HasColumnName(@"PlannedEndDate").HasColumnType("datetime").IsRequired();
            Property(x => x.ActualEndDate).HasColumnName(@"ActualEndDate").HasColumnType("datetime").IsOptional();
            Property(x => x.CompletionAmount).HasColumnName(@"CompletionAmount").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            Property(x => x.MonthlyInstallment).HasColumnName(@"MonthlyInstallment").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            Property(x => x.NumberOfInstallments).HasColumnName(@"NumberOfInstallments").HasColumnType("smallint").IsRequired();
            InitializePartial();
        }
        partial void InitializePartial();
    }
}