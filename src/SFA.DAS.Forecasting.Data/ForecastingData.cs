﻿

// ------------------------------------------------------------------------------------------------
// This code was generated by EntityFramework Reverse POCO Generator (http://www.reversepoco.com/).
// Created by Simon Hughes (https://about.me/simon.hughes).
//
// Do not make changes directly to this file - edit the template instead.
//
// The following connection settings were used to generate this file:
//     Configuration file:     "SFA.DAS.Forecasting.Data\App.config"
//     Connection String Name: "DatabaseConnectionString"
//     Connection String:      "Data Source=.;Initial Catalog=SFA.DAS.Forecasting.Database;Integrated Security=True;Pooling=False;Connect Timeout=30"
// ------------------------------------------------------------------------------------------------
// Database Edition       : Developer Edition (64-bit)
// Database Engine Edition: Enterprise

// <auto-generated>
// ReSharper disable ConvertPropertyToExpressionBody
// ReSharper disable DoNotCallOverridableMethodsInConstructor
// ReSharper disable EmptyNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable PartialMethodWithSinglePart
// ReSharper disable PartialTypeWithSinglePart
// ReSharper disable RedundantNameQualifier
// ReSharper disable RedundantOverridenMember
// ReSharper disable UseNameofExpression
// TargetFrameworkVersion = 4.6

using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.SqlServer;
using SFA.DAS.Forecasting.Core;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;

#pragma warning disable 1591    //  Ignore "Missing XML Comment" warning


namespace SFA.DAS.Forecasting.Data
{

    #region Unit of work

    public partial interface IForecastingDataContext : System.IDisposable
    {
        System.Data.Entity.DbSet<AccountProjection> AccountProjections { get; set; } // AccountProjection
        System.Data.Entity.DbSet<AccountProjectionCommitment> AccountProjectionCommitments { get; set; } // AccountProjectionCommitment
        System.Data.Entity.DbSet<BalanceModel> Balances { get; set; } // Balance
        System.Data.Entity.DbSet<CommitmentModel> Commitments { get; set; } // Commitment
        System.Data.Entity.DbSet<FundingSource> FundingSources { get; set; } // FundingSource
        System.Data.Entity.DbSet<LevyDeclaration> LevyDeclarations { get; set; } // LevyDeclaration
        System.Data.Entity.DbSet<Payment> Payments { get; set; } // Payment

        int SaveChanges();
        System.Threading.Tasks.Task<int> SaveChangesAsync();
        System.Threading.Tasks.Task<int> SaveChangesAsync(System.Threading.CancellationToken cancellationToken);
        System.Data.Entity.Infrastructure.DbChangeTracker ChangeTracker { get; }
        System.Data.Entity.Infrastructure.DbContextConfiguration Configuration { get; }
        System.Data.Entity.Database Database { get; }
        System.Data.Entity.Infrastructure.DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        System.Data.Entity.Infrastructure.DbEntityEntry Entry(object entity);
        System.Collections.Generic.IEnumerable<System.Data.Entity.Validation.DbEntityValidationResult> GetValidationErrors();
        System.Data.Entity.DbSet Set(System.Type entityType);
        System.Data.Entity.DbSet<TEntity> Set<TEntity>() where TEntity : class;
        string ToString();
    }

    #endregion

    #region Database context
    public class ForecastingDbConfiguration : DbConfiguration
    {
        public ForecastingDbConfiguration()
        {
            SetProviderServices("System.Data.EntityClient",
                SqlProviderServices.Instance);
            SetDefaultConnectionFactory(new SqlConnectionFactory());
        }
    }

    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    [DbConfigurationType(typeof(ForecastingDbConfiguration))]
    public partial class ForecastingDataContext : System.Data.Entity.DbContext, IForecastingDataContext
    {
        public System.Data.Entity.DbSet<AccountProjection> AccountProjections { get; set; } // AccountProjection
        public System.Data.Entity.DbSet<AccountProjectionCommitment> AccountProjectionCommitments { get; set; } // AccountProjectionCommitment
        public System.Data.Entity.DbSet<BalanceModel> Balances { get; set; } // Balance
        public System.Data.Entity.DbSet<CommitmentModel> Commitments { get; set; } // Commitment
        public System.Data.Entity.DbSet<FundingSource> FundingSources { get; set; } // FundingSource
        public System.Data.Entity.DbSet<LevyDeclaration> LevyDeclarations { get; set; } // LevyDeclaration
        public System.Data.Entity.DbSet<Payment> Payments { get; set; } // Payment

        static ForecastingDataContext()
        {
            System.Data.Entity.Database.SetInitializer<ForecastingDataContext>(null);
        }

        public ForecastingDataContext()
            : base("Name=DatabaseConnectionString")
        {
            InitializePartial();
        }

        public ForecastingDataContext(IApplicationConnectionStrings config) 
            : base(config.DatabaseConnectionString)
        {
            InitializePartial();
             
        }

        public ForecastingDataContext(string connectionString)
            : base(connectionString)
        {
            InitializePartial();
        }

        public ForecastingDataContext(string connectionString, System.Data.Entity.Infrastructure.DbCompiledModel model)
            : base(connectionString, model)
        {
            InitializePartial();
        }

        public ForecastingDataContext(System.Data.Common.DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            InitializePartial();
        }

        public ForecastingDataContext(System.Data.Common.DbConnection existingConnection, System.Data.Entity.Infrastructure.DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            InitializePartial();
        }

        protected override void Dispose(bool disposing)
        {
            DisposePartial(disposing);
            base.Dispose(disposing);
        }

        public bool IsSqlParameterNull(System.Data.SqlClient.SqlParameter param)
        {
            var sqlValue = param.SqlValue;
            var nullableValue = sqlValue as System.Data.SqlTypes.INullable;
            if (nullableValue != null)
                return nullableValue.IsNull;
            return (sqlValue == null || sqlValue == System.DBNull.Value);
        }

        protected override void OnModelCreating(System.Data.Entity.DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.Add(new AccountProjectionConfiguration());
            modelBuilder.Configurations.Add(new AccountProjectionCommitmentConfiguration());
            modelBuilder.Configurations.Add(new BalanceConfiguration());
            modelBuilder.Configurations.Add(new CommitmentConfiguration());
            modelBuilder.Configurations.Add(new FundingSourceConfiguration());
            modelBuilder.Configurations.Add(new LevyDeclarationConfiguration());
            modelBuilder.Configurations.Add(new PaymentConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        public static System.Data.Entity.DbModelBuilder CreateModel(System.Data.Entity.DbModelBuilder modelBuilder, string schema)
        {
            modelBuilder.Configurations.Add(new AccountProjectionConfiguration(schema));
            modelBuilder.Configurations.Add(new AccountProjectionCommitmentConfiguration(schema));
            modelBuilder.Configurations.Add(new BalanceConfiguration(schema));
            modelBuilder.Configurations.Add(new CommitmentConfiguration(schema));
            modelBuilder.Configurations.Add(new FundingSourceConfiguration(schema));
            modelBuilder.Configurations.Add(new LevyDeclarationConfiguration(schema));
            modelBuilder.Configurations.Add(new PaymentConfiguration(schema));
            return modelBuilder;
        }

        partial void InitializePartial();
        partial void DisposePartial(bool disposing);
        partial void OnModelCreatingPartial(System.Data.Entity.DbModelBuilder modelBuilder);
    }
    #endregion

    #region Database context factory

    public partial class ForecastingDataContextFactory : System.Data.Entity.Infrastructure.IDbContextFactory<ForecastingDataContext>
    {
        public ForecastingDataContext Create()
        {
            return new ForecastingDataContext();
        }
    }

    #endregion

    #region POCO classes

    // AccountProjection
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class AccountProjection
    {
        public long Id { get; set; } // Id (Primary key)
        public long EmployerAccountId { get; set; } // EmployerAccountId
        public System.DateTime ProjectionCreationDate { get; set; } // ProjectionCreationDate
        public short ProjectionGenerationType { get; set; } // ProjectionGenerationType
        public short Month { get; set; } // Month
        public int Year { get; set; } // Year
        public decimal FundsIn { get; set; } // FundsIn
        public decimal TotalCostOfTraining { get; set; } // TotalCostOfTraining
        public decimal CompletionPayments { get; set; } // CompletionPayments
        public decimal FutureFunds { get; set; } // FutureFunds
        public decimal CoInvestmentEmployer { get; set; } // CoInvestmentEmployer
        public decimal CoInvestmentGovernment { get; set; } // CoInvestmentGovernment

        // Reverse navigation

        /// <summary>
        /// Child AccountProjectionCommitments where [AccountProjectionCommitment].[AccountProjectionId] point to this entity (FK_AccountProjectionCommitment__AccountProjection)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<AccountProjectionCommitment> AccountProjectionCommitments { get; set; } // AccountProjectionCommitment.FK_AccountProjectionCommitment__AccountProjection

        public AccountProjection()
        {
            CoInvestmentEmployer = 0m;
            CoInvestmentGovernment = 0m;
            AccountProjectionCommitments = new System.Collections.Generic.List<AccountProjectionCommitment>();
            InitializePartial();
        }

        partial void InitializePartial();
    }

    // AccountProjectionCommitment
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class AccountProjectionCommitment
    {
        public long Id { get; set; } // Id (Primary key)
        public long AccountProjectionId { get; set; } // AccountProjectionId
        public long CommitmentId { get; set; } // CommitmentId

        // Foreign keys

        /// <summary>
        /// Parent AccountProjection pointed by [AccountProjectionCommitment].([AccountProjectionId]) (FK_AccountProjectionCommitment__AccountProjection)
        /// </summary>
        public virtual AccountProjection AccountProjection { get; set; } // FK_AccountProjectionCommitment__AccountProjection

        /// <summary>
        /// Parent Commitment pointed by [AccountProjectionCommitment].([CommitmentId]) (FK_AccountProjectionCommitment__Commitment)
        /// </summary>
        public virtual CommitmentModel Commitment { get; set; } // FK_AccountProjectionCommitment__Commitment

        public AccountProjectionCommitment()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

    // Balance
    //[System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    //public partial class Balance
    //{
    //    public long EmployerAccountId { get; set; } // EmployerAccountId (Primary key)
    //    public decimal Amount { get; set; } // Amount
    //    public decimal TransferAllowance { get; set; } // TransferAllowance
    //    public decimal RemainingTransferBalance { get; set; } // RemainingTransferBalance
    //    public System.DateTime BalancePeriod { get; set; } // BalancePeriod
    //    public System.DateTime ReceivedDate { get; set; } // ReceivedDate

    //    public Balance()
    //    {
    //        Amount = 0m;
    //        TransferAllowance = 0m;
    //        RemainingTransferBalance = 0m;
    //        ReceivedDate = System.DateTime.Now;
    //        InitializePartial();
    //    }

    //    partial void InitializePartial();
    //}

    // Commitment
    //[System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    //public partial class Commitment
    //{
    //    public long Id { get; set; } // Id (Primary key)
    //    public long EmployerAccountId { get; set; } // EmployerAccountId
    //    public long LearnerId { get; set; } // LearnerId
    //    public long ProviderId { get; set; } // ProviderId
    //    public string ProviderName { get; set; } // ProviderName (length: 200)
    //    public long ApprenticeshipId { get; set; } // ApprenticeshipId
    //    public string ApprenticeName { get; set; } // ApprenticeName (length: 200)
    //    public string CourseName { get; set; } // CourseName (length: 200)
    //    public int? CourseLevel { get; set; } // CourseLevel
    //    public System.DateTime StartDate { get; set; } // StartDate
    //    public System.DateTime PlannedEndDate { get; set; } // PlannedEndDate
    //    public System.DateTime? ActualEndDate { get; set; } // ActualEndDate
    //    public decimal CompletionAmount { get; set; } // CompletionAmount
    //    public decimal MonthlyInstallment { get; set; } // MonthlyInstallment
    //    public short NumberOfInstallments { get; set; } // NumberOfInstallments

    //    // Reverse navigation

    //    /// <summary>
    //    /// Child AccountProjectionCommitments where [AccountProjectionCommitment].[CommitmentId] point to this entity (FK_AccountProjectionCommitment__Commitment)
    //    /// </summary>
    //    public virtual System.Collections.Generic.ICollection<AccountProjectionCommitment> AccountProjectionCommitments { get; set; } // AccountProjectionCommitment.FK_AccountProjectionCommitment__Commitment

    //    public Commitment()
    //    {
    //        AccountProjectionCommitments = new System.Collections.Generic.List<AccountProjectionCommitment>();
    //        InitializePartial();
    //    }

    //    partial void InitializePartial();
    //}

    // FundingSource
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class FundingSource
    {
        public byte Id { get; set; } // Id (Primary key)
        public string Name { get; set; } // Name (length: 200)

        // Reverse navigation

        /// <summary>
        /// Child Payments where [Payment].[FundingSource] point to this entity (FK_Payment__FundingSource)
        /// </summary>
        public virtual System.Collections.Generic.ICollection<Payment> Payments { get; set; } // Payment.FK_Payment__FundingSource

        public FundingSource()
        {
            Payments = new System.Collections.Generic.List<Payment>();
            InitializePartial();
        }

        partial void InitializePartial();
    }

    // LevyDeclaration
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class LevyDeclaration
    {
        public long Id { get; set; } // Id (Primary key)
        public long EmployerAccountId { get; set; } // EmployerAccountId
        public string Scheme { get; set; } // Scheme (length: 50)
        public string PayrollYear { get; set; } // PayrollYear (length: 10)
        public byte PayrollMonth { get; set; } // PayrollMonth
        public System.DateTime PayrollDate { get; set; } // PayrollDate
        public decimal LevyAmountDeclared { get; set; } // LevyAmountDeclared
        public System.DateTime TransactionDate { get; set; } // TransactionDate
        public System.DateTime DateReceived { get; set; } // DateReceived

        public LevyDeclaration()
        {
            DateReceived = System.DateTime.Now;
            InitializePartial();
        }

        partial void InitializePartial();
    }

    // Payment
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class Payment
    {
        public long Id { get; set; } // Id (Primary key)
        public string ExternalPaymentId { get; set; } // ExternalPaymentId (length: 100)
        public long EmployerAccountId { get; set; } // EmployerAccountId
        public long ProviderId { get; set; } // ProviderId
        public long ApprenticeshipId { get; set; } // ApprenticeshipId
        public decimal Amount { get; set; } // Amount
        public System.DateTime ReceivedTime { get; set; } // ReceivedTime
        public long LearnerId { get; set; } // LearnerId
        public int CollectionPeriodMonth { get; set; } // CollectionPeriodMonth
        public int CollectionPeriodYear { get; set; } // CollectionPeriodYear
        public int DeliveryPeriodMonth { get; set; } // DeliveryPeriodMonth
        public int DeliveryPeriodYear { get; set; } // DeliveryPeriodYear
        public byte FundingSource { get; set; } // FundingSource

        // Foreign keys

        /// <summary>
        /// Parent FundingSource pointed by [Payment].([FundingSource]) (FK_Payment__FundingSource)
        /// </summary>
        public virtual FundingSource FundingSource_FundingSource { get; set; } // FK_Payment__FundingSource

        public Payment()
        {
            InitializePartial();
        }

        partial void InitializePartial();
    }

    #endregion

    #region POCO Configuration

    // AccountProjection
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class AccountProjectionConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<AccountProjection>
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

    // AccountProjectionCommitment
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class AccountProjectionCommitmentConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<AccountProjectionCommitment>
    {
        public AccountProjectionCommitmentConfiguration()
            : this("dbo")
        {
        }

        public AccountProjectionCommitmentConfiguration(string schema)
        {
            ToTable("AccountProjectionCommitment", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").HasColumnType("bigint").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity);
            Property(x => x.AccountProjectionId).HasColumnName(@"AccountProjectionId").HasColumnType("bigint").IsRequired();
            Property(x => x.CommitmentId).HasColumnName(@"CommitmentId").HasColumnType("bigint").IsRequired();

            // Foreign keys
            HasRequired(a => a.AccountProjection).WithMany(b => b.AccountProjectionCommitments).HasForeignKey(c => c.AccountProjectionId).WillCascadeOnDelete(false); // FK_AccountProjectionCommitment__AccountProjection
            //HasRequired(a => a.Commitment).WithMany(b => b.AccountProjectionCommitments).HasForeignKey(c => c.CommitmentId).WillCascadeOnDelete(false); // FK_AccountProjectionCommitment__Commitment
            InitializePartial();
        }
        partial void InitializePartial();
    }

    // Balance
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

    // Commitment
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

    // FundingSource
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class FundingSourceConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<FundingSource>
    {
        public FundingSourceConfiguration()
            : this("dbo")
        {
        }

        public FundingSourceConfiguration(string schema)
        {
            ToTable("FundingSource", schema);
            HasKey(x => x.Id);

            Property(x => x.Id).HasColumnName(@"Id").HasColumnType("tinyint").IsRequired().HasDatabaseGeneratedOption(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.None);
            Property(x => x.Name).HasColumnName(@"Name").HasColumnType("nvarchar").IsRequired().HasMaxLength(200);
            InitializePartial();
        }
        partial void InitializePartial();
    }

    // LevyDeclaration
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class LevyDeclarationConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<LevyDeclaration>
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
            Property(x => x.EmployerAccountId).HasColumnName(@"EmployerAccountId").HasColumnType("bigint").IsRequired();
            Property(x => x.Scheme).HasColumnName(@"Scheme").HasColumnType("nvarchar").IsRequired().HasMaxLength(50);
            Property(x => x.PayrollYear).HasColumnName(@"PayrollYear").HasColumnType("nvarchar").IsRequired().HasMaxLength(10);
            Property(x => x.PayrollMonth).HasColumnName(@"PayrollMonth").HasColumnType("tinyint").IsRequired();
            Property(x => x.PayrollDate).HasColumnName(@"PayrollDate").HasColumnType("datetime").IsRequired();
            Property(x => x.LevyAmountDeclared).HasColumnName(@"LevyAmountDeclared").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            Property(x => x.TransactionDate).HasColumnName(@"TransactionDate").HasColumnType("datetime").IsRequired();
            Property(x => x.DateReceived).HasColumnName(@"DateReceived").HasColumnType("datetime").IsRequired();
            InitializePartial();
        }
        partial void InitializePartial();
    }

    // Payment
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class PaymentConfiguration : System.Data.Entity.ModelConfiguration.EntityTypeConfiguration<Payment>
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
            Property(x => x.Amount).HasColumnName(@"Amount").HasColumnType("decimal").IsRequired().HasPrecision(18,2);
            Property(x => x.ReceivedTime).HasColumnName(@"ReceivedTime").HasColumnType("datetime").IsRequired();
            Property(x => x.LearnerId).HasColumnName(@"LearnerId").HasColumnType("bigint").IsRequired();
            Property(x => x.CollectionPeriodMonth).HasColumnName(@"CollectionPeriodMonth").HasColumnType("int").IsRequired();
            Property(x => x.CollectionPeriodYear).HasColumnName(@"CollectionPeriodYear").HasColumnType("int").IsRequired();
            Property(x => x.DeliveryPeriodMonth).HasColumnName(@"DeliveryPeriodMonth").HasColumnType("int").IsRequired();
            Property(x => x.DeliveryPeriodYear).HasColumnName(@"DeliveryPeriodYear").HasColumnType("int").IsRequired();
            Property(x => x.FundingSource).HasColumnName(@"FundingSource").HasColumnType("tinyint").IsRequired();

            // Foreign keys
            HasRequired(a => a.FundingSource_FundingSource).WithMany(b => b.Payments).HasForeignKey(c => c.FundingSource).WillCascadeOnDelete(false); // FK_Payment__FundingSource
            InitializePartial();
        }
        partial void InitializePartial();
    }

    #endregion

}
// </auto-generated>

