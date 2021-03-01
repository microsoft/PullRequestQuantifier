namespace PullRequestQuantifier.Repository.Service.Models
{
    using Microsoft.WindowsAzure.Storage.Table;

    public sealed class CommitStats : TableEntity
    {
        public string CommitSha1 { get; set; }

        public int QuantifiedLinesAdded { get; set; }

        public int QuantifiedLinesDeleted { get; set; }

        public float PercentileAddition { get; set; }

        public float PercentileDeletion { get; set; }

        public float DiffPercentile { get; set; }

        public string Label { get; set; }

        public int AbsoluteLinesAdded { get; set; }

        public int AbsoluteLinesDeleted { get; set; }
    }
}
