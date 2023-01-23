namespace SFA.DAS.Forecasting.Core.Configuration;

public class ForecastingJobsConfiguration
{
    public string CosmosDbConnectionString { get; set; }
    public string EmployerConnectionString { get; set; }
    public bool AllowTriggerProjections { get; set; }
    public const int NumberOfMonthsToProject = 24;
    public string ForecastingConnectionString { get; set; }
    public string StorageConnectionString { get; set; }
}