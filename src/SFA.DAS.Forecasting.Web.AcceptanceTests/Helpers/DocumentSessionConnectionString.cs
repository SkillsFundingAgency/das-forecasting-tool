using System.Data.Common;

namespace SFA.DAS.Forecasting.Web.AcceptanceTests.Helpers
{
    public class DocumentSessionConnectionString : DbConnectionStringBuilder
    {
        public string Database { get => (string)this["Database"]; set => this["Database"] = value; }
        public string AccountEndpoint { get => (string)this["AccountEndpoint"]; set => this["AccountEndpoint"] = value; }
        public string AccountKey { get => (string)this["AccountKey"]; set => this["AccountKey"] = value; }
        public string Collection { get => (string)this["Collection"]; set => this["Collection"] = value; }
        public string ThroughputOffer { get => (string)this["ThroughputOffer"]; set => this["ThroughputOffer"] = value; }
    }
}
