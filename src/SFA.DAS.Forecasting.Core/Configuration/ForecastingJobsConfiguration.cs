namespace SFA.DAS.Forecasting.Core.Configuration;

public class ForecastingJobsConfiguration
{
    public string EmployerConnectionString { get; set; }
    public bool AllowTriggerProjections { get; set; }
    public const int NumberOfMonthsToProject = 24;
    
    public string StorageConnectionString { get; set; }
}