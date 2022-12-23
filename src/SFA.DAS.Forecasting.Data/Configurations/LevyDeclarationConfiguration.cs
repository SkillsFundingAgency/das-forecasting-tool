using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SFA.DAS.Forecasting.Models.Levy;

namespace SFA.DAS.Forecasting.Data.Configurations
{
    public class LevyDeclarationConfiguration : IEntityTypeConfiguration<LevyDeclarationModel>
    {
        public void Configure(EntityTypeBuilder<LevyDeclarationModel> builder)
        {
            builder.ToTable("LevyDeclaration");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().ValueGeneratedOnAdd();
            builder.Property(x => x.SubmissionId).HasColumnName(@"SubmissionId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").HasColumnType("bigint").IsRequired();
            builder.Property(x => x.Scheme).HasColumnName(@"Scheme").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            builder.Property(x => x.PayrollYear).HasColumnName(@"PayrollYear").HasColumnType("nvarchar").IsRequired().HasMaxLength(10);
            builder.Property(x => x.PayrollMonth).HasColumnName(@"PayrollMonth").HasColumnType("tinyint").IsRequired();
            builder.Property(x => x.PayrollDate).HasColumnName(@"PayrollDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.LevyAmountDeclared).HasColumnName(@"LevyAmountDeclared").HasColumnType("decimal").IsRequired().HasPrecision(18,5);
            builder.Property(x => x.TransactionDate).HasColumnName(@"TransactionDate").HasColumnType("datetime").IsRequired();
            builder.Property(x => x.DateReceived).HasColumnName(@"DateReceived").HasColumnType("datetime").IsRequired();
        }
    }
}