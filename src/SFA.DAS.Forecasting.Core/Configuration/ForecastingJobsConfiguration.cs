namespace SFA.DAS.Forecasting.Core.Configuration;

public class ForecastingJobsConfiguration
{
    public string CosmosDbConnectionString { get; set; }
    public string EmployerConnectionString { get; set; }
    public bool AllowTriggerProjections { get; set; }
    public int SecondsToWaitToAllowProjections { get; set; }
    public int NumberOfMonthsToProject { get; set; }
    public string ForecastingConnectionString { get; set; }
    public string StorageConnectionString { get; set; }
}