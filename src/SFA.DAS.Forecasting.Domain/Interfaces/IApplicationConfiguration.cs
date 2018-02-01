namespace SFA.DAS.Forecasting.Domain.Interfaces
{
    public interface IApplicationConfiguration
    {
        string DatabaseConnectionString { get; set; }
        string BackLink { get; set; }
    }
}
