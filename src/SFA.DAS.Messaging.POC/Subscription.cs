namespace SFA.DAS.Messaging.POC
{
    public class Subscription
    {
        public string MessageTypeName { get; set; }
        public string QueueName { get; set; }
        //TODO: add storage account details here to allow sends between apps using different accounts
    }
}