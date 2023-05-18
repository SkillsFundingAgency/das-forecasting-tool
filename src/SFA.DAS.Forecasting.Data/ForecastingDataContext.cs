using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SFA.DAS.Forecasting.Core.Configuration;
using SFA.DAS.Forecasting.Data.Configurations;
using SFA.DAS.Forecasting.Models.Balance;
using SFA.DAS.Forecasting.Models.Commitments;
using SFA.DAS.Forecasting.Models.Levy;
using SFA.DAS.Forecasting.Models.Payments;
using SFA.DAS.Forecasting.Models.Projections;

namespace SFA.DAS.Forecasting.Data
{
    public interface IForecastingDataContext
    {
        DbSet<AccountProjectionModel> AccountProjections { get; set; } 
        DbSet<AccountProjectionCommitment> AccountProjectionCommitments { get; set; } 
        DbSet<BalanceModel> Balances { get; set; } 
        DbSet<CommitmentModel> Commitments { get; set; } 
        DbSet<LevyDeclarationModel> LevyDeclarations { get; set; } // LevyDeclaration
        DbSet<PaymentModel> Payments { get; set; } // Payment
        int SaveChanges();
        Task<int> ExecuteRawSql(string sql);
        Task<int> ExecuteSqlInterpolatedAsync(FormattableString sql);
    }

    public class ForecastingDataContext : DbContext, IForecastingDataContext
    {
        private const string AzureResource = "https://database.windows.net/";
        
        public DbSet<AccountProjectionModel> AccountProjections { get; set; } 
        public DbSet<AccountProjectionCommitment> AccountProjectionCommitments { get; set; }
        public DbSet<BalanceModel> Balances { get; set; } 
        public DbSet<CommitmentModel> Commitments { get; set; } 
        public DbSet<LevyDeclarationModel> LevyDeclarations { get; set; }
        public DbSet<PaymentModel> Payments { get; set; }
        private readonly ForecastingConnectionStrings _configuration;
        private readonly ChainedTokenCredential _azureServiceTokenProvider;
     
        public ForecastingDataContext()
        {
        }

        public ForecastingDataContext(DbContextOptions options) : base(options)
        {
            
        }
        public ForecastingDataContext(IOptions<ForecastingConnectionStrings> config, DbContextOptions options, ChainedTokenCredential azureServiceTokenProvider) :base(options)
        {
            _configuration = config.Value;
            _azureServiceTokenProvider = azureServiceTokenProvider;
        }
        public async Task<int> ExecuteRawSql(string sql)
        {
            var result = await Database.ExecuteSqlRawAsync(sql);
            return result;
        }

        public async Task<int> ExecuteSqlInterpolatedAsync(FormattableString sql)
        {
            var result = await Database.ExecuteSqlInterpolatedAsync(sql);
            return result;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();

            if (_configuration == null || _azureServiceTokenProvider == null)
            {
                optionsBuilder.UseSqlServer().UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                return;
            }
            
            var connection = new SqlConnection
            {
                ConnectionString = _configuration.DatabaseConnectionString,
                AccessToken = _azureServiceTokenProvider.GetTokenAsync(new TokenRequestContext(scopes: new string[] { AzureResource })).Result.Token,
            };
            
            optionsBuilder.UseSqlServer(connection,options=>
                options.EnableRetryOnFailure(
                    5,
                    TimeSpan.FromSeconds(20),
                    null
                )).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            

            modelBuilder.ApplyConfiguration(new AccountProjectionConfiguration());
            modelBuilder.ApplyConfiguration(new AccountProjectionCommitmentConfiguration());
            modelBuilder.ApplyConfiguration(new BalanceConfiguration());
            modelBuilder.ApplyConfiguration(new CommitmentConfiguration());
            modelBuilder.ApplyConfiguration(new LevyDeclarationConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}