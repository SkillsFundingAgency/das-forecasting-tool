using System.Threading.Tasks;
using Dapper;
using SFA.DAS.Forecasting.Payments.Domain.Entities;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Sql.Client;

namespace SFA.DAS.Forecasting.Payments.Application.Infrastructure
{
    public class PaymentDbRepository : BaseRepository
    {
        public PaymentDbRepository(string databaseConnectionString, ILog logger)
            : base(databaseConnectionString, logger)
        {
        }

        public async Task InsertOrUpdatePayment(PaymentApprenticeship record)
        {
            await WithConnection(
                async c =>
                {
                    string sql = "INSERT INTO Apprenticeship" +
                    "(EmployerAccountId, Name, DateOfBirth, TrainingName, TrainingLevel, TrainingProviderName, StartDate, MonthlyPayment, Instalments, CompletionPayment) " +
                    "values(@EmployerAccountId, @Name, @DateOfBirth, @TrainingName, @TrainingLevel, @TrainingProviderName, @StartDate, @MonthlyPayment, @Instalments, @CompletionPayment);";

                    var result =
                        await 
                        c.ExecuteAsync(
                              sql
                            , record);
                    return 1;
                });
        }
    }
}
