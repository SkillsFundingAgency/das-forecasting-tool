namespace SFA.DAS.Forecasting.Data
{
    public partial class ForecastingDataContextFactory : System.Data.Entity.Infrastructure.IDbContextFactory<ForecastingDataContext>
    {
        public ForecastingDataContext Create()
        {
            return new ForecastingDataContext();
        }
    }
}