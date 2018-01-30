using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Forecasting.Application.Payments.Repositories.Models
{
    public class TableEntry : TableEntity
    {
        public string Data { get; set; }
    }
}
