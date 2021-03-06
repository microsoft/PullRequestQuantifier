namespace PullRequestQuantifier.Repository.Service.Models
{
    using CsvHelper.Configuration;

    public sealed class CommitStatsMap : ClassMap<CommitStats>
    {
        public CommitStatsMap()
        {
            Map(m => m.CommitSha1);
            Map(m => m.QuantifiedLinesAdded);
            Map(m => m.QuantifiedLinesDeleted);
            Map(m => m.PercentileAddition);
            Map(m => m.PercentileDeletion);
            Map(m => m.DiffPercentile);
            Map(m => m.Label);
            Map(m => m.AbsoluteLinesAdded);
            Map(m => m.AbsoluteLinesDeleted);
        }
    }
}
