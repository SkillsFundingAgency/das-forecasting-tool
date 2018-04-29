namespace SFA.DAS.Forecasting.Core
{
    public interface IApplicationConnectionStrings
    {
        string DatabaseConnectionString { get; }
        string StorageConnectionString { get; }
        string EmployerConnectionString { get; }
    }
}