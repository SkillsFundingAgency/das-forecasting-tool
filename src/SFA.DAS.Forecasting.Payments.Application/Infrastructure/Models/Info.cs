using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.Forecasting.Payments.Application.Infrastructure.Models
{
    public class Info : TableEntity
    {
        public int PageNumber { get; set; }
    }

    public class TableRow : TableEntity
    {
        public string Data { get; set; }
    }
}
