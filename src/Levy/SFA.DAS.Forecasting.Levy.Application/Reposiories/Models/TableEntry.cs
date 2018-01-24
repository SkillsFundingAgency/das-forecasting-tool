using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace SFA.DAS.Forecasting.Levy.Application.Reposiories.Models
{
    public class TableEntry : TableEntity
    {
        public string Data { get; set; }
    }
}
