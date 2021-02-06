namespace PullRequestQuantifier.Feedback.Service.Models
{
    using Microsoft.WindowsAzure.Storage.Table;

    public class FeedbackModel : TableEntity
    {
        public FeedbackModel(
            string partitionKey,
            string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }

        public FeedbackModel()
        {
        }

        public string Text { get; set; }

        public string EventType { get; set; }

        public string NotNormalizedPartitionKey { get; set; }
    }
}
