namespace SFA.DAS.Forecasting.Core.Configuration
{
	public class Config : IConfig
	{
		public string StorageConnectionString { get; set; }

        public string LevyDeclarationTableName { get; set; }
    }
}
