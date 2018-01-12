using Microsoft.WindowsAzure.Storage.Table;

namespace SFA.DAS.PaymentsAdapter.Functions.Infrastructure.Models
{
    public class Info : TableEntity
    {
        public int PageNumber { get; set; }
    }
}
