using SFA.DAS.Forecasting.Data.Configurations;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Levy;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Data
{
    public partial interface IForecastingDataContext : System.IDisposable
    {
        //System.Data.Entity.DbSet<AccountProjectionMonth> AccountProjections { get; set; } // AccountProjection
        //System.Data.Entity.DbSet<AccountProjectionCommitment> AccountProjectionCommitments { get; set; } // AccountProjectionCommitment
        System.Data.Entity.DbSet<BalanceModel> Balances { get; set; } // Balance
        System.Data.Entity.DbSet<CommitmentModel> Commitments { get; set; } // Commitment
        //System.Data.Entity.DbSet<FundingSource> FundingSources { get; set; } // FundingSource
        System.Data.Entity.DbSet<LevyDeclarationModel> LevyDeclarations { get; set; } // LevyDeclaration
        System.Data.Entity.DbSet<PaymentModel> Payments { get; set; } // Payment

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

    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.36.1.0")]
    public partial class ForecastingDataContext : System.Data.Entity.DbContext, IForecastingDataContext
    {
        //public System.Data.Entity.DbSet<AccountProjectionMonth> AccountProjections { get; set; } // AccountProjection
        //public System.Data.Entity.DbSet<AccountProjectionCommitment> AccountProjectionCommitments { get; set; } // AccountProjectionCommitment
        public System.Data.Entity.DbSet<BalanceModel> Balances { get; set; } // Balance
        public System.Data.Entity.DbSet<CommitmentModel> Commitments { get; set; } // Commitment
        //public System.Data.Entity.DbSet<FundingSource> FundingSources { get; set; } // FundingSource
        public System.Data.Entity.DbSet<LevyDeclarationModel> LevyDeclarations { get; set; } // LevyDeclaration
        public System.Data.Entity.DbSet<PaymentModel> Payments { get; set; } // Payment

        static ForecastingDataContext()
        {
            System.Data.Entity.Database.SetInitializer<ForecastingDataContext>(null);
        }

        public ForecastingDataContext()
            : base("Name=DatabaseConnectionString")
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

            //modelBuilder.Configurations.Add(new AccountProjectionConfiguration());
            //modelBuilder.Configurations.Add(new AccountProjectionCommitmentConfiguration());
            modelBuilder.Configurations.Add(new BalanceConfiguration());
            modelBuilder.Configurations.Add(new CommitmentConfiguration());
            //modelBuilder.Configurations.Add(new FundingSourceConfiguration());
            modelBuilder.Configurations.Add(new LevyDeclarationConfiguration());
            modelBuilder.Configurations.Add(new PaymentConfiguration());

            OnModelCreatingPartial(modelBuilder);
        }

        public static System.Data.Entity.DbModelBuilder CreateModel(System.Data.Entity.DbModelBuilder modelBuilder, string schema)
        {
            //modelBuilder.Configurations.Add(new AccountProjectionConfiguration(schema));
            //modelBuilder.Configurations.Add(new AccountProjectionCommitmentConfiguration(schema));
            modelBuilder.Configurations.Add(new BalanceConfiguration(schema));
            modelBuilder.Configurations.Add(new CommitmentConfiguration(schema));
            //modelBuilder.Configurations.Add(new FundingSourceConfiguration(schema));
            modelBuilder.Configurations.Add(new LevyDeclarationConfiguration(schema));
            modelBuilder.Configurations.Add(new PaymentConfiguration(schema));
            return modelBuilder;
        }

        partial void InitializePartial();
        partial void DisposePartial(bool disposing);
        partial void OnModelCreatingPartial(System.Data.Entity.DbModelBuilder modelBuilder);
    }
}