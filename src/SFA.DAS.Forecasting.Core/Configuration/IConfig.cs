namespace SFA.DAS.Forecasting.Core.Configuration
{
	public interface IConfig
	{
		string StorageConnectionString { get; set; }
        string LevyDeclarationTableName { get; }
    }
}