namespace PullRequestQuantifier.Repository.Service.Models
{
    using System.Globalization;
    using CsvHelper.Configuration;

    public sealed class CommitStatsMap : ClassMap<CommitStats>
    {
        public CommitStatsMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.PartitionKey).Ignore();
            Map(m => m.RowKey).Ignore();
            Map(m => m.Timestamp).Ignore();
            Map(m => m.ETag).Ignore();
        }
    }
}
