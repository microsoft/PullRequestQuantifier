namespace PullRequestQuantifier.Repository.Service.Models
{
    using System;
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

        public long PullRequestId { get; set; }

        public string PullRequestAuthor { get; set; }

        public TimeSpan PullRequestLeadTime { get; set; }

        // Workaround the fact that TableEntity does not automatically store a `TimeSpan`
        public long PullRequestLeadTimeTicks
        {
            get => PullRequestLeadTime.Ticks;
            set => PullRequestLeadTime = TimeSpan.FromTicks(value);
        }
    }
}
