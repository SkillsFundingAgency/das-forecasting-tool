using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Forecasting.Levy.Application.Services.Models
{
    public class TableEntry : TableEntity
    {
        public string Data { get; set; }
    }
}
