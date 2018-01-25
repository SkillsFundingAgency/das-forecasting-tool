using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Forecasting.AcceptanceTests.Services.Models
{
    public class TableRow : TableEntity
    {
        public string Data { get; set; }
    }
}