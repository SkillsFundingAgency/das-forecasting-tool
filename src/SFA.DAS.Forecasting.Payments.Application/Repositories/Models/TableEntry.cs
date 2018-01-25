using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Forecasting.Payments.Application.Repositories.Models
{
    public class TableEntry : TableEntity
    {
        public string Data { get; set; }
    }
}
