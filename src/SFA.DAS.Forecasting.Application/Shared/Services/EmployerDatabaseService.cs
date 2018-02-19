using Dapper;
using SFA.DAS.Forecasting.Application.Infrastructure.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.Forecasting.Application.Shared.Services
{
    public interface IEmployerDatabaseService
    {
        // ToDo: move
        Task<IEnumerable<EmployerPayment>> GetEmployerPayments(long accountId, int year, int month);
    }

    public class EmployerDatabaseService : BaseRepository, IEmployerDatabaseService
    {
        public EmployerDatabaseService(
            IApplicationConfiguration config,
            ILog logger)
            : base(config.EmployerConnectionString, logger)
        {
            //_logger = logger;
        }

        public async Task<IEnumerable<EmployerPayment>> GetEmployerPayments(long accountId, int year, int month)
        {
            // Get all Payments where AccountId and  ,CollectionPeriodMonth ,CollectionPeriodYear --> 10005694, 5, 2017
            var sql = "SELECT" +
                        "[PaymentId],[Ukprn],[Uln],[AccountId],[ApprenticeshipId] " +
                        ",[CollectionPeriodId],[CollectionPeriodMonth],[CollectionPeriodYear],[Amount],[PaymentMetaDataId],[ProviderName] " +
                        ",[StandardCode],[FrameworkCode],[ProgrammeType],[PathwayCode],[PathwayName] " +
                        ",[ApprenticeshipCourseName],[ApprenticeshipCourseStartDate],[ApprenticeshipCourseLevel],[ApprenticeName] " +
                    "FROM [employer_financial].[Payment] " +
                    "inner join [employer_financial].[PaymentMetaData] metaData " +
                    "on payment.PaymentMetaDataId = metaData.Id " +
                    "where AccountId = @employerAccountId " + 
                    "and CollectionPeriodYear = @year " +  
                    "and CollectionPeriodMonth = @month ";

            // ToDo: Can there be more than one row per AccountId, CollectionPeriodMonth and CollectionPeriodYear?
            return await WithConnection(async cnn =>
            {
                var parameters = new DynamicParameters();
                parameters.Add("@employerAccountId", accountId, DbType.Int64);
                parameters.Add("@year", year, DbType.Int32);
                parameters.Add("@month", month, DbType.Int32);

                var levyDeclarations = await cnn.QueryAsync<EmployerPayment>(
                        sql,
                            parameters,
                            commandType: CommandType.Text);
                return levyDeclarations.ToList();
            });

            //var message = 
            //    new PaymentCreatedMessage
            //    {
            //         Id= "", // PaymentId?
            //        EmployerAccountId = 1234, // OK froim DB
            //        Ukprn = 123445, // OK from DB
            //        ApprenticeshipId = 222, // OK from DB
            //        Amount = 200, // OK from DB
            //        ProviderName = "", // OK from DB PaymentMetaData
            //        ApprenticeName = "", // OK from DB PaymentMetaData
            //        CourseName = "", // OK from DB PaymentMetaData
            //        CourseLevel = 1, // OK from DB PaymentMetaData
            //        Uln = 123456789, // OK from DB
            //        CourseStartDate = null, // OK from DB PaymentMetaData

            //        //EarningDetails EarningDetails= "", // From Payments
            //        CollectionPeriod= new CollectionPeriod // ??
            //        {
            //            Id = "",
            //            Year = year,
            //            Month = month 
            //        },

            //        // Payment --> PaymentMetaDataId --> PaymentMetaData
            //    };
        }
    }

    public class EmployerPayment
    {
        public Guid PaymentId;
        public long Ukprn;
        public long Uln;
        public long AccountId;
        public long ApprenticeshipId;
        public string CollectionPeriodId;
        public int CollectionPeriodMonth;
        public int CollectionPeriodYear;
        public decimal Amount;
        public long PaymentMetaDataId;
        public string ProviderName;
        public int StandardCode;
        public int FrameworkCode;
        public int ProgrammeType;
        public int PathwayCode;
        public string PathwayName;
        public string ApprenticeshipCourseName;
        public DateTime? ApprenticeshipCourseStartDate;
        public int ApprenticeshipCourseLevel;
        public string ApprenticeName;
    }
}
